using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class SetManagerApplicationDto
    {
        public Guid ApplicationId { get; set; }
        public Guid ManagerId { get; set; }
    }
}
