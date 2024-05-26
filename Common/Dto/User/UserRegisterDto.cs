using Common.Enums;


namespace Common.Dto.User
{
    public class UserRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Lastname { get; set; }
        public required string Middlename { get; set; }
        public required string Firstname { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
