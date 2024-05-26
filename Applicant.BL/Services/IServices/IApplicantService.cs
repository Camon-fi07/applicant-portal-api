using Common.Dto.Applicant;

namespace Applicant.BL.Services.IServices
{
    public interface IApplicantService
    {
        public Task RegisterApplicant(RegisterApplicantDto registerApplicantDto);
        public Task<ApplicantDataDto> EditApplicantData(EditApplicantDataDto editApplicantDataDto, Guid userId);
        public Task<ApplicantDataDto> GetApplicantData(Guid userId);
        public Task<PassportDto> GetPassport(Guid userId);
        public Task<PassportDto> EditPassport(EditPassportDto editPassportDto, Guid userId);
        public Task<EducationDocumentDto> GetEducationDocument(Guid userId);
        public Task<EducationDocumentDto> EditEducationDocument(EditEducationDocumentDto editEducationDocumentDto, Guid userId);
    }
}
