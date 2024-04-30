using Applicant.DAL.Entities;
using Common.Enums;
using Common.Models;

namespace Applicant.BL.Dto
{
    public class RegisterApplicantDto
    {
        public string Citizenship { get; set; }
        public CreatePassportDto Passport { get; set; }
        public Guid UserId { get; set; }
        public CreateEducationDocumentDto Education { get; set; }
    }
}
