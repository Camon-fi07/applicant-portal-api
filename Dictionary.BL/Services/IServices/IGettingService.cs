using Common.Dto;
using Common.Dto.Dictionary;
using Dictionary.BL.Dto;
using Dictionary.BL.Models;
using Dictionary.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.BL.Services.IServices
{
    public interface IGettingService
    {
        public Task<List<EducationLevelDto>> GetEducationLevels(string? name);
        public Task<EducationDocumentTypeDto> GetDocumentType(Guid id);
        public Task<List<EducationDocumentTypeDto>> GetDocumentTypes(string? name, int? educationLevelId);
        public Task<List<EducationFacultyDto>> GetFaculties(string? name);
        public Task<ProgramPagedListModel> GetPrograms(int page, int size, Guid? facultyId, int? educationLevelId, string? educationForm, string? language, string? name, string? code);
        public Task<List<ImportingStatusDto>> GetImportingStatuses();
    }
}
