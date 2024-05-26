using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class SetManagerRoleDto
    {
        public Guid id { get; set; }
        public ManagerRole Role { get; set; }
    }
}
