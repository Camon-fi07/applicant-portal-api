using Common.Enums;
using Common.Models;

namespace Common.Dto
{
    public class RegisterApplicantDto:UserRegisterModel
    {
        public string Citizenship { get; set; }
    }
}
