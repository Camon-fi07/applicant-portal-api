using Applicant.DAL.Entities;
using Common.Enums;
using Common.Models;

namespace Common.Dto
{
    public class RegisterApplicantDto
    {
        public string Citizenship { get; set; }
        public PassportEntity Passport { get; set; }
        public Guid UserId { get; set; }
        public EducationEntity Education { get; set; }
    }
}
