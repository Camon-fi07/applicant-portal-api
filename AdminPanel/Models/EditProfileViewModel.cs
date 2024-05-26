using Common.Enums;

namespace AdminPanel.Models
{
    public class EditProfileViewModel
    {
        public string? Email { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? BirthDate { get; set; }
        public Gender? Gender { get; set; }
    }
}
