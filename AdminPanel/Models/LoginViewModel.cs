using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ожидалось значение")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Ожидалось значение")]
    [DataType(DataType.Password)]
    [Display(Name = "Email")]
    public string Password { get; set; }
}
