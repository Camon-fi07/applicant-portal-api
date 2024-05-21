using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dto.User;

namespace Common.Dto.Applicant
{
    public class ApplicantProfileDto:ProfileDto
    {
        public string Citizenship { get; set; }
        public PassportDto Passport { get; set; }
        public EducationDocumentDto EducationDocument { get; set; }
    }
}
