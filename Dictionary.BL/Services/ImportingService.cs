using Common.Constants;
using Common.Dto.Dictionary;
using Common.Enums;
using Dictionary.BL.Mappers;
using Dictionary.BL.Models;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL;
using Dictionary.DAL.Entities;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;


namespace Dictionary.BL.Services
{
    public class ImportingService:IImportingService
    {
        DictionaryDBContext _dictionaryDBContext;
        HttpClient _httpClient;
        IConfiguration _configuration; // TODO IOptions чтобы не кофигать каждый раз
        RemoteDictionaryModel _remoteDictionary;
        IBus _bus;

        public ImportingService(DictionaryDBContext dictionaryDBContext, HttpClient httpClient, IConfiguration configuration, IBus bus) {
            _dictionaryDBContext = dictionaryDBContext;
            _httpClient = httpClient;
            _configuration = configuration;
            _remoteDictionary = new RemoteDictionaryModel();
            _configuration.GetSection("RemoteDictionary").Bind(_remoteDictionary);
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_remoteDictionary.Login}:{_remoteDictionary.Password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            _bus = bus;
        }

        public async Task ImporDocumentTypes()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/document_types");

            if (!httpResponse.IsSuccessStatusCode) {
                await AddImportingStatus((int)httpResponse.StatusCode, "Произошла ошибка при запросе", GuideType.DocumentTypes);

                throw new Exception("Произошла ошибка");
            };

            var documentTypes = JsonConvert.DeserializeObject<List<EducationDocumentTypeDto>>(await httpResponse.Content.ReadAsStringAsync());

            foreach (var document in documentTypes)
            {
                var currentDocument = await _dictionaryDBContext.EducationDocumentTypes.FirstOrDefaultAsync(d => d.Id == document.Id);
                if (currentDocument != null) _dictionaryDBContext.EducationDocumentTypes.Remove(currentDocument);

                await _dictionaryDBContext.SaveChangesAsync();

                var currentLevel = _dictionaryDBContext.EducationLevels.FirstOrDefault(l => l.Id == document.EducationLevel.Id);

                if (currentLevel == null) currentLevel = EducationLevelMapper.TranslateLevelDtoToEntity(document.EducationLevel);

                var nextEducationLevels = new List<EducationLevelEntity>();

                foreach (var levelEntity in document.NextEducationLevels)
                {
                    var nextLevel = _dictionaryDBContext.EducationLevels.FirstOrDefault(l => l.Id == levelEntity.Id);
                    nextEducationLevels.Add(nextLevel ?? EducationLevelMapper.TranslateLevelDtoToEntity(levelEntity));
                }

                _dictionaryDBContext.EducationDocumentTypes.Add(EducationDocumentTypeMapper.TranslateDocumnetDtoToEntity(document, currentLevel, nextEducationLevels));
            }

            await _dictionaryDBContext.SaveChangesAsync();

            await AddImportingStatus((int)httpResponse.StatusCode, "Всё прошло успешно", GuideType.DocumentTypes);

            await _bus.PubSub.PublishAsync(GuideType.DocumentTypes, x => x.WithTopic(RabbitMQQueue.REFRESH_GUIDE));
        }

        public async Task ImportEducationLevels()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/education_levels");

            if (!httpResponse.IsSuccessStatusCode) {
                await AddImportingStatus((int)httpResponse.StatusCode, "Произошла ошибка при запросе", GuideType.EducationLevels);

                throw new Exception("Произошла ошибка");
            };

            var educationLevels = JsonConvert.DeserializeObject<List<EducationLevelDto>>(await httpResponse.Content.ReadAsStringAsync());

            educationLevels = educationLevels.Where(e => e.Id != 6).ToList();

            var originEducationLevels = _dictionaryDBContext.EducationLevels.Include(l => l.EducationPrograms).Include(l => l.CurrentEducationLevelDocumentTypes);

            await originEducationLevels.ForEachAsync((originLevel) =>
            {
                var newLevel = educationLevels?.FirstOrDefault(l => l.Id == originLevel.Id);

                if (newLevel == null)
                {
                    _dictionaryDBContext.EducationLevels.Remove(originLevel);
                    if(originLevel.CurrentEducationLevelDocumentTypes != null) _dictionaryDBContext.EducationDocumentTypes.RemoveRange(originLevel.CurrentEducationLevelDocumentTypes);
                    if(originLevel.EducationPrograms != null) _dictionaryDBContext.EducationPrograms.RemoveRange(originLevel.EducationPrograms);
                }
                else
                {
                    originLevel.Name = newLevel.Name;
                }
            });
            
            foreach(var level in educationLevels)
            {
                var currentLevel = await _dictionaryDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == level.Id);
                if (currentLevel == null) _dictionaryDBContext.EducationLevels.Add(EducationLevelMapper.TranslateLevelDtoToEntity(level));
            }

            await _dictionaryDBContext.SaveChangesAsync();
            await AddImportingStatus((int)httpResponse.StatusCode, "Всё прошло успешно", GuideType.EducationLevels);

            await _bus.PubSub.PublishAsync(GuideType.EducationLevels, x => x.WithTopic(RabbitMQQueue.REFRESH_GUIDE));
        }

        public async Task ImportFaculties()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/faculties");

            if (!httpResponse.IsSuccessStatusCode) {
                await AddImportingStatus((int)httpResponse.StatusCode, "Произошла ошибка при запросе", GuideType.Faculties);

                throw new Exception("Произошла ошибка");
            };

            var faculties = JsonConvert.DeserializeObject<List<EducationFacultyDto>>(await httpResponse.Content.ReadAsStringAsync());
            var originFaculties = _dictionaryDBContext.Faculties.Include(f => f.EducationPrograms);

            await originFaculties.ForEachAsync((originFaculty) =>
            {
                var newFaculty = faculties?.FirstOrDefault(f => f.Id == originFaculty.Id);

                if (newFaculty == null)
                {
                    _dictionaryDBContext.Faculties.Remove(originFaculty);
                    if (originFaculty.EducationPrograms != null) _dictionaryDBContext.EducationPrograms.RemoveRange(originFaculty.EducationPrograms);
                }
                else
                {
                    originFaculty.Name = newFaculty.Name;
                    originFaculty.CreateTime = newFaculty.CreateTime.ToUniversalTime();
                }
            });

            foreach (var faculty in faculties)
            {
                var currentFaculty = await _dictionaryDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == faculty.Id);
                if (currentFaculty == null) _dictionaryDBContext.Faculties.Add(FacultyMapper.TranslateFacultyDtoToEntity(faculty));
            }

            await _dictionaryDBContext.SaveChangesAsync();
            await AddImportingStatus((int)httpResponse.StatusCode, "Всё прошло успешно", GuideType.Faculties);

            await _bus.PubSub.PublishAsync(GuideType.Faculties, x => x.WithTopic(RabbitMQQueue.REFRESH_GUIDE));
        }

        public async Task ImportPrograms()
        {
            string queryParams = "page=1&size=1000000";
            string requestURL = $"{_remoteDictionary.URL}/programs?{queryParams}";

            var httpResponse = await _httpClient.GetAsync(requestURL);

            if (!httpResponse.IsSuccessStatusCode) {
                await AddImportingStatus((int)httpResponse.StatusCode, "Произошла ошибка при запросе", GuideType.EducationPrograms);

                throw new Exception("Произошла ошибка");
            };

            var programsPagedListModel = JsonConvert.DeserializeObject<ProgramPagedListModel>(await httpResponse.Content.ReadAsStringAsync());
            var programs = programsPagedListModel.Programs;

            foreach (var program in programs)
            {
                var currentProgram = await _dictionaryDBContext.EducationPrograms.FirstOrDefaultAsync(p => p.Id == program.Id);
                if (currentProgram != null) _dictionaryDBContext.EducationPrograms.Remove(currentProgram);

                var currentLevel = _dictionaryDBContext.EducationLevels.FirstOrDefault(l => l.Id == program.EducationLevel.Id);
                if(currentLevel == null) currentLevel = new EducationLevelEntity() { Id = program.EducationLevel.Id, Name = program.EducationLevel.Name };

                var currentFaculty = _dictionaryDBContext.Faculties.FirstOrDefault(f => f.Id == program.Faculty.Id);
                if (currentFaculty == null) currentFaculty = new FacultyEntity() { Id = program.Faculty.Id, Name = program.Faculty.Name, CreateTime = program.Faculty.CreateTime };

                _dictionaryDBContext.EducationPrograms.Add(new EducationProgramEntity()
                {
                    Id = program.Id,
                    EducationLevel = currentLevel,
                    Code = program.Code,
                    CreateTime = program.CreateTime.ToUniversalTime(),
                    EducationForm = program.EducationForm,
                    Faculty = currentFaculty,
                    Language = program.Language,
                    Name = program.Name,
                });
            }

            await _dictionaryDBContext.SaveChangesAsync();
            await AddImportingStatus((int)httpResponse.StatusCode, "Всё прошло успешно", GuideType.EducationPrograms);

            await _bus.PubSub.PublishAsync(GuideType.EducationPrograms, x => x.WithTopic(RabbitMQQueue.REFRESH_GUIDE));
        }

        private async Task AddImportingStatus(int statusCode, string description, GuideType guideType)
        {
            _dictionaryDBContext.ImportingStatuses.Add(new ImportingStatusEntity()
            {
                CreateTime = DateTime.UtcNow,
                StatusCode = statusCode,
                GuideType = guideType,
                Id = Guid.NewGuid(),
                Description = description
            });

            await _dictionaryDBContext.SaveChangesAsync();
        }
    }
}
