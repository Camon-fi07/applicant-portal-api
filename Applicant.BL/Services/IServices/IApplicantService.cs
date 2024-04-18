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
        public Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerApplicantDto);
        public Task<ApplicantProfileDto> GetApplicantProfile();
        public Task<TokenResponseDto> EditApplicantProfile(EditApplicantProfileDto editApplicantProfileDto);
    }
}
