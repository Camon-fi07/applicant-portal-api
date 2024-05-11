using Common.Dto.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Dto
{
    public class CreateApplicationDto
    {
        public CreatePassportDto Passport { get; set; }
        public CreateEducationDocumentDto Education { get; set; }
        public List<ProgramPriorityDto> Programs { get; set; }
        public string Citizenship { get; set; }
    }
}
