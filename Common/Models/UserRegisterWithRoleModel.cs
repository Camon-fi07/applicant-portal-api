using Common.Enums;

namespace Common.Models
{
    public class UserRegisterWithRoleModel:UserRegisterModel
    {
        public RoleType Role { get; set; }
    }
}
