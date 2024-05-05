using EasyNetQ;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    [Queue("documentDelete")]
    public class DeleteSingleFileDto
    {
        public Guid FileId { get; set; }
    }

    [Queue("documentDelete")]
    public class DeleteFilesDto
    {
        public List<Guid> FilesIds { get; set; }
    }
}
