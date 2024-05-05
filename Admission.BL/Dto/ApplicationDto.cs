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
        public ApplicantProfileDto User { get; set; }
        public PassportDto Passport { get; set; }
        public EducationDocumentDto Education { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
