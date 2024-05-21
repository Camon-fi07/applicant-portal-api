using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dto.User;

namespace Common.Dto.Applicant
{
    public class EditApplicantProfileDto:EditProfileDto
    {
        public string? Citizenship { get; set; }
        public EditPassportDto Passport { get; set; }
        public EditEducationDocumentDto EducationDocument { get; set; }
    }
}
