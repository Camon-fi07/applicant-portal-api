using EasyNetQ;
using Microsoft.AspNetCore.Http;

namespace Common.Dto.RabbitMQ
{
    [Queue("documentUpload")]
    public class UploadSingleFileDto
    {
        public IFormFile File { get; set; }
    }

    [Queue("documentUpload")]
    public class UploadFilesDto
    {
        public List<IFormFile> File { get; set; }
    }
}
