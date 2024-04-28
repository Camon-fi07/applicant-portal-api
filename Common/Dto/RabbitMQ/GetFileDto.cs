using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    [Queue("documentGet")]
    public class GetFileDto
    {
        public Guid Id { get; set; }
    }
}
