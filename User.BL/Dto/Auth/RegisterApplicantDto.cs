using Common.Enums;
using User.BL.Models;
namespace User.BL.Dto.Auth
{
    public class RegisterApplicantDto:UserRegisterModel
    {
        public string Citizenship { get; set; }
    }
}
