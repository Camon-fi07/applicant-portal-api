using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class LogoutRabbitDto
    {
        public Guid UserId { get; set; }
        public string UserAgent { get; set; }
    }
}
