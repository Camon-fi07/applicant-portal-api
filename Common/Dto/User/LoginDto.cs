using System.ComponentModel.DataAnnotations;

namespace Common.Dto.User
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
