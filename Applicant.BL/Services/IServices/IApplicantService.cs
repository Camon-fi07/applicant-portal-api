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
        public Task<ApplicantProfileDto> GetProfile();
        public Task GetPasport();
        public Task EditPassport();
        public Task DeletePassport();
        public Task GetEducationDocument();
        public Task EditEducationDocument();
        public Task DeleteEducationDocument();
        public Task<TokenResponseDto> EditProfile(EditApplicantProfileDto editApplicantProfileDto);
    }
}
