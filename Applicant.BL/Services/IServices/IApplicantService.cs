using Applicant.BL.Dto;

namespace Applicant.BL.Services.IServices
{
    public interface IApplicantService
    {
        public Task RegisterApplicant(RegisterApplicantDto registerApplicantDto);
        public Task<ApplicantProfileDto> EditProfile(EditApplicantProfileDto editApplicantProfileDto, Guid userId);
        public Task<ApplicantProfileDto> GetProfile(Guid userId);
        public Task<PassportDto> GetPassport(Guid userId);
        public Task<PassportDto> EditPassport(EditPassportDto editPassportDto, Guid userId);
        public Task<EducationDocumentDto> GetEducationDocument(Guid userId);
        public Task<EducationDocumentDto> EditEducationDocument(EditEducationDocumentDto editEducationDocumentDto, Guid userId);
    }
}
