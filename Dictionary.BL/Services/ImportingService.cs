using Dictionary.BL.Dto;
using Dictionary.BL.Mappers;
using Dictionary.BL.Models;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL;
using Dictionary.DAL.Entities;
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
        IConfiguration _configuration;
        RemoteDictionaryModel _remoteDictionary;

        public ImportingService(DictionaryDBContext dictionaryDBContext, HttpClient httpClient, IConfiguration configuration) {
            _dictionaryDBContext = dictionaryDBContext;
            _httpClient = httpClient;
            _configuration = configuration;
            _remoteDictionary = new RemoteDictionaryModel();
            _configuration.GetSection("RemoteDictionary").Bind(_remoteDictionary);
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_remoteDictionary.Login}:{_remoteDictionary.Password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        public async Task ImporDocumentTypes()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/document_types");

            if (!httpResponse.IsSuccessStatusCode) throw new Exception("Произошла ошибка");

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
        }

        public async Task ImportEducationLevels()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/education_levels");

            if (!httpResponse.IsSuccessStatusCode) throw new Exception("Произошла ошибка");

            var educationLevels = JsonConvert.DeserializeObject<List<EducationLevelDto>>(await httpResponse.Content.ReadAsStringAsync());
            
            foreach(var level in educationLevels)
            {
                var currentLevel = await _dictionaryDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == level.Id);
                if (currentLevel != null) _dictionaryDBContext.EducationLevels.Remove(currentLevel);

                _dictionaryDBContext.EducationLevels.Add(EducationLevelMapper.TranslateLevelDtoToEntity(level));
            }

            await _dictionaryDBContext.SaveChangesAsync();
        }

        public async Task ImportFaculties()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/faculties");

            if (!httpResponse.IsSuccessStatusCode) throw new Exception("Произошла ошибка");

            var faculties = JsonConvert.DeserializeObject<List<EducationFacultyDto>>(await httpResponse.Content.ReadAsStringAsync());

            foreach (var faculty in faculties)
            {
                var currentFaculty = await _dictionaryDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == faculty.Id);
                if (currentFaculty != null) _dictionaryDBContext.Faculties.Remove(currentFaculty);
                faculty.CreateTime = faculty.CreateTime.ToUniversalTime();
                _dictionaryDBContext.Faculties.Add(new FacultyEntity()
                {
                    Id = faculty.Id,
                    Name=faculty.Name,
                    CreateTime = faculty.CreateTime.ToUniversalTime()
                });
            }

            await _dictionaryDBContext.SaveChangesAsync();
        }

        public async Task ImportPrograms()
        {
            string queryParams = "page=1&size=1000000";
            string requestURL = $"{_remoteDictionary.URL}/programs?{queryParams}";

            var httpResponse = await _httpClient.GetAsync(requestURL);

            if (!httpResponse.IsSuccessStatusCode) throw new Exception("Произошла ошибка");

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
        }
    }
}
