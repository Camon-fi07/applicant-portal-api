using EasyNetQ;
using Microsoft.AspNetCore.Http;

namespace Common.Dto.RabbitMQ
{
    [Queue("documentUpload")]
    public class UploadFileDto
    {
        public IFormFile File { get; set; }
    }
}
