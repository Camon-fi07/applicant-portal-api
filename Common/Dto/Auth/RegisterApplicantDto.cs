using Common.Enums;

namespace Common.Dto.Auth
{
    public class RegisterApplicantDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Firstname { get; set; }
        public Gender Gender { get; set; }
        public string Citizenship { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
