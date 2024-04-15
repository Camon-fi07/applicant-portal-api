using Dictionary.BL.Models;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL;
using Dictionary.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;


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

            var documentTypes = JsonConvert.DeserializeObject<List<EducationDocumentTypeEntity>>(await httpResponse.Content.ReadAsStringAsync());
            
            foreach (var document in documentTypes)
            {
                var currentDocument = await _dictionaryDBContext.EducationDocumentTypes.FirstOrDefaultAsync(d => d.Id == document.Id);
                if(currentDocument != null) _dictionaryDBContext.EducationDocumentTypes.Remove(currentDocument);

                await _dictionaryDBContext.SaveChangesAsync();

                var currentLevel = _dictionaryDBContext.EducationLevels.FirstOrDefault(l => l.Id == document.EducationLevel.Id);
                var nextEducationLevels = new List<EducationLevelEntity>();

                foreach(var levelEntity in document.NextEducationLevels)
                {
                    var nextLevel = _dictionaryDBContext.EducationLevels.FirstOrDefault(l => l.Id == document.EducationLevel.Id);
                    nextEducationLevels.Add(nextLevel ?? levelEntity);
                }
                document.EducationLevel = currentLevel ?? document.EducationLevel;
                document.CreateTime = document.CreateTime.ToUniversalTime();
                document.NextEducationLevels = nextEducationLevels;

                _dictionaryDBContext.EducationDocumentTypes.Add(document);
            }

            await _dictionaryDBContext.SaveChangesAsync();
        }

        public async Task ImportEducationLevels()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/education_levels");

            if (!httpResponse.IsSuccessStatusCode) throw new Exception("Произошла ошибка");

            var educationLevels = JsonConvert.DeserializeObject<List<EducationLevelEntity>>(await httpResponse.Content.ReadAsStringAsync());
            
            foreach(var level in educationLevels)
            {
                var currentLevel = await _dictionaryDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == level.Id);
                if (currentLevel != null) _dictionaryDBContext.EducationLevels.Remove(currentLevel);

                _dictionaryDBContext.EducationLevels.Add(level);
            }

            await _dictionaryDBContext.SaveChangesAsync();
        }

        public async Task ImportFaculties()
        {
            var httpResponse = await _httpClient.GetAsync($"{_remoteDictionary.URL}/faculties");

            if (!httpResponse.IsSuccessStatusCode) throw new Exception("Произошла ошибка");

            var faculties = JsonConvert.DeserializeObject<List<FacultyEntity>>(await httpResponse.Content.ReadAsStringAsync());

            foreach (var faculty in faculties)
            {
                var currentFaculty = await _dictionaryDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == faculty.Id);
                if (currentFaculty != null) _dictionaryDBContext.Faculties.Remove(currentFaculty);
                faculty.CreateTime = faculty.CreateTime.ToUniversalTime();
                _dictionaryDBContext.Faculties.Add(faculty);
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
                var currentFaculty = _dictionaryDBContext.Faculties.FirstOrDefault(f => f.Id == program.Faculty.Id);
                program.CreateTime = program.CreateTime.ToUniversalTime();
                program.Faculty = currentFaculty ?? program.Faculty;
                program.EducationLevel = currentLevel ?? program.EducationLevel;
                _dictionaryDBContext.EducationPrograms.Add(program);
            }

            await _dictionaryDBContext.SaveChangesAsync();
        }
    }
}
