using Common.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class LoginRabbitDto
    {
        public string UserAgent { get; set; }
        public LoginDto LoginDto { get; set; }
    }
}
