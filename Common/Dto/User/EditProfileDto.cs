using Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Common.Dto.User
{
    public class EditProfileDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; }
        [RegularExpression(@"^\+7 \([0-9]{3}\) [0-9]{3} [0-9]{2} [0-9]{2}$")]
        [AllowNull]
        public string? PhoneNumber { get; set; }
        public DateOnly? BirthDate { get; set; }
        public Gender? Gender { get; set; }
    }
}
