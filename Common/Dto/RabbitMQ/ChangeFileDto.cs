using EasyNetQ;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    [Queue("documentChange")]
    public class ChangeFileDto
    {
        public IFormFile File { get; set; }
        public Guid FileId { get; set; }
    }
}
