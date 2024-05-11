using Common.Dto;
using Common.Dto.Applicant;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Dto
{
    public class ApplicationDto
    {
        public List<ProgramPriorityDto> ProgramPriorities { get; set; }
        public Guid ApplicationId { get; set; }
        public ApplicantDataDto Applicant { get; set; }
        public UserDto User { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
