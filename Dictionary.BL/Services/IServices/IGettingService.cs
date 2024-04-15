using Dictionary.BL.Dto;
using Dictionary.BL.Models;
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
        public Task<List<EducationDocumentTypeDto>> GetDocumentTypes(string? name, int? educationLevelId);
        public Task<List<EducationFacultyDto>> GetFaculties(string? name);
        public Task<ProgramPagedListModel> GetPrograms(int page, int size, Guid? facultyId, int? educationLevelId, string? educationForm, string? language, string? name, string? code);
    }
}
