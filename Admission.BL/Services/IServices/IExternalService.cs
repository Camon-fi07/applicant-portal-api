using Admission.DAL.Entities;

namespace Admission.BL.Services.IServices
{
    public interface IExternalService
    { 
        public Task<UserCacheEntity> GetUser(Guid userId);
        public Task ImportUser(Guid userId);
        public Task<ProgramCacheEntity> GetProgram(Guid programId);
        public Task ImportProgram(Guid programId);
        public Task<FacultyCacheEntity> GetFaculty(Guid facultyId);
        public Task ImportFaculty(Guid facultyId);
        public Task<EducationDocumentTypeCacheEntity> GetEducationDocument(Guid documentId);
        public Task ImportEducationDocument(Guid documentId);
    }
}
