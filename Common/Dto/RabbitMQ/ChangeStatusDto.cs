using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class ChangeStatusDto
    {
        public ApplicationStatus Status { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
