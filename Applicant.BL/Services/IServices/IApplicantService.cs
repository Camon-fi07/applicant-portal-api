using Applicant.BL.Dto;
using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Services.IServices
{
    public interface IApplicantService
    {
        public Task RegisterApplicant(RegisterApplicantDto registerApplicantDto);
        public Task<TokenResponseDto> EditProfile(EditApplicantProfileDto editApplicantProfileDto, Guid userId);
        public Task<ApplicantProfileDto> GetProfile(Guid userId);
        public Task<PassportDto> GetPasport();
        public Task EditPassport(EditPassportDto editPassportDto);
        public Task<EducationDocumentDto> GetEducationDocument();
        public Task EditEducationDocument(EditEducationDocumentDto editEducationDocumentDto);
    }
}
