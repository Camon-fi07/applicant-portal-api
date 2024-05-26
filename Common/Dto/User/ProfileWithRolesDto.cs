using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.User
{
    public class ProfileWithRolesDto:ProfileDto
    {
        public List<RoleType> Roles { get; set; }
    }
}
