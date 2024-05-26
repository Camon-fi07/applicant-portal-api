using Common.Dto.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAdminPanelApplicantService
    {
        public Task<ApplicantDataDto> EditApplicantData(EditApplicantDataDto editApplicantDataDto, Guid userId);
        public Task<ApplicantDataDto> GetApplicantData(Guid userId);
    }
}
