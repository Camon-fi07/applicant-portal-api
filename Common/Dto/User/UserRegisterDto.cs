using Common.Enums;
using System.ComponentModel.DataAnnotations;


namespace Common.Dto.User
{
    public class UserRegisterDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
        [RegularExpression(@"^\+7 \([0-9]{3}\) [0-9]{3} [0-9]{2} [0-9]{2}$")]
        public required string PhoneNumber { get; set; }
        public required string Lastname { get; set; }
        public required string Middlename { get; set; }
        public required string Firstname { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
