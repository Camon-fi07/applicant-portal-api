using Common.Enums;

namespace User.BL.Dto.Profile
{
    public class ProfileDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public List<RoleType?> Roles { get; set; }
    }
}
