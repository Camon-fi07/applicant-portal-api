using Dictionary.DAL.Entities;

namespace Dictionary.BL.Services.IServices
{
    public interface IImportingService
    {
        public Task ImportEducationLevels();
        public Task ImporDocumentTypes();
        public Task ImportFaculties();
        public Task ImportPrograms();
        public Task<List<ImportingStatusEntity>> GetImportingStatuses();
    }
}
