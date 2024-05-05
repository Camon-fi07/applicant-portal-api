using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    [Queue("documentGet")]
    public class GetSingleFileDto
    {
        public Guid Id { get; set; }
    }

    [Queue("documentGet")]
    public class GetFilesDto
    {
        public List<Guid> Ids { get; set; }
    }
}
