using Common.Models;
using Dictionary.BL.Dto;
using Dictionary.BL.Mappers;
using Dictionary.BL.Models;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Dictionary.BL.Services
{
    public class GettingService:IGettingService
    {
        DictionaryDBContext _dictionaryDBContext;

        public GettingService(DictionaryDBContext dictionaryDBContext, HttpClient httpClient, IConfiguration configuration)
        {
            _dictionaryDBContext = dictionaryDBContext;
        }

        public async Task<List<EducationDocumentTypeDto>> GetDocumentTypes(string? name, int? educationLevelId)
        {
            var documentTypes = _dictionaryDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).Include(d => d.NextEducationLevels).ToList();
            if (educationLevelId != null) documentTypes = documentTypes.Where(d => d.EducationLevel.Id == educationLevelId).ToList();
            if(name != null) documentTypes = documentTypes.Where(d => d.Name.ToLower().StartsWith(name.ToLower())).ToList();
            return documentTypes.Select(d => EducationDocumentTypeMapper.TranslateDocumentEntityToDto(d)).ToList();
        }

        public async Task<List<EducationLevelDto>> GetEducationLevels(string? name)
        {
            var levels = _dictionaryDBContext.EducationLevels.ToList();
            if(name != null) levels = levels.Where(l => l.Name.ToLower().StartsWith(name.ToLower())).ToList();
            return levels.Select(l => EducationLevelMapper.TranslateLevelEntityToDto(l)).ToList();
        }
        
        public async Task<List<EducationFacultyDto>> GetFaculties(string? name)
        {
            var faculties = _dictionaryDBContext.Faculties.ToList();
            if (name != null) faculties = faculties.Where(f => f.Name.ToLower().StartsWith(name.ToLower())).ToList();
            return faculties.Select(f => new EducationFacultyDto()
            {
                CreateTime = f.CreateTime.ToUniversalTime(),
                Id = f.Id,
                Name = f.Name
            }).ToList();
        }

        public async Task<ProgramPagedListModel> GetPrograms(int page, int size, Guid? facultyId, int? educationLevelId, string? educationForm, string? language, string? name, string? code)
        {
            var programs = _dictionaryDBContext.EducationPrograms.Include(p => p.Faculty).Include(p => p.EducationLevel).ToList();
            if (facultyId != null) programs = programs.Where(p => p.Faculty.Id == facultyId).ToList();
            if (educationLevelId != null) programs = programs.Where(p => p.EducationLevel.Id == educationLevelId).ToList();
            if (educationForm != null) programs = programs.Where(p => p.EducationForm.ToLower().StartsWith(educationForm.ToLower())).ToList();
            if (language != null) programs = programs.Where(p => p.Language.ToLower().StartsWith(language.ToLower())).ToList();
            if (name != null) programs = programs.Where(p => p.Name.ToLower().StartsWith(name.ToLower())).ToList();
            if (code != null) programs = programs.Where(p => p.Code == null || p.Code.StartsWith(code)).ToList();
            int count = programs.Count() / (page * size) == 0 ? 1 : programs.Count() / (page * size);
            programs = programs.Skip((page-1) * size).Take(size).ToList();

            return new ProgramPagedListModel()
            {
                Pagination = new PageInfoModel()
                {
                    Count = count,
                    Current = page,
                    Size = size
                },
                Programs = programs.Select(p => new EducationProgramDto()
                {
                    Code = p.Code,
                    CreateTime = p.CreateTime.ToUniversalTime(),
                    EducationForm = p.EducationForm,
                    EducationLevel = EducationLevelMapper.TranslateLevelEntityToDto(p.EducationLevel),
                    Faculty = new EducationFacultyDto()
                    {
                        CreateTime = p.Faculty.CreateTime.ToUniversalTime(),
                        Id = p.Faculty.Id,
                        Name = p.Faculty.Name
                    },
                    Id = p.Id,
                    Language = p.Language,
                    Name = p.Name,
                }).ToList()
            };


        }
    }
}
