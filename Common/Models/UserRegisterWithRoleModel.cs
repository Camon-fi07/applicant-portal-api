using Common.Dto.User;
using Common.Enums;

namespace Common.Models
{
    public class UserRegisterWithRoleModel:UserRegisterDto
    {
        public RoleType Role { get; set; }
    }
}
