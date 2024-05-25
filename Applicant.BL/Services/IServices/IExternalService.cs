using Applicant.DAL.Entities;

namespace Applicant.BL.Services.IServices
{
    public interface IExternalService
    {
        public Task<EducationDocumentTypeCacheEntity> GetEducationDocument(Guid documentId);
        public Task ImportEducationDocument(Guid documentId);
    }
}
