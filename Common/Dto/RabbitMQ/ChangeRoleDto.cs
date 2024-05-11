using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class ChangeRoleDto
    {
        public RoleType Role { get; set; }
        public Guid UserId { get; set; }
    }
}
