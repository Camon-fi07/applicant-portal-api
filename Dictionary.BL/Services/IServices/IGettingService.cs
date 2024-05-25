using Common.Dto;
using Common.Dto.Dictionary;
using Common.Dto.RabbitMQ;
using Dictionary.BL.Models;

namespace Dictionary.BL.Services.IServices
{
    public interface IGettingService
    {
        public Task<List<EducationLevelDto>> GetEducationLevels(string? name);
        public Task<List<AddEducationLevelDto>> SychronizeEducationLevels(List<int> ids);
        public Task<EducationDocumentTypeDto> GetCertainDocumentType(Guid id);
        public Task<List<AddEducationDocumentTypeDto>> SychronizeDocumentTypes(List<Guid> ids);
        public Task<List<EducationDocumentTypeDto>> GetDocumentTypes(string? name, int? educationLevelId);
        public Task<List<EducationFacultyDto>> GetFaculties(string? name);
        public Task<EducationFacultyDto> GetCertainFaculty(Guid id);
        public Task<List<AddFacultyDto>> SynchronizeFaculties(List<Guid> ids);
        public Task<ProgramPagedListModel> GetPrograms(int page, int size, Guid? facultyId, int? educationLevelId, string? educationForm, string? language, string? name, string? code);
        public Task<EducationProgramDto> GetCertainProgram(Guid id);
        public Task<List<AddProgramDto>> SychronizePrograms(List<Guid> ids);
        public Task<List<ImportingStatusDto>> GetImportingStatuses();
    }
}
