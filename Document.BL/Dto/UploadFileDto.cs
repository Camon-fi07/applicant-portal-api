using Microsoft.AspNetCore.Http;

namespace Document.BL.Dto
{
    public class UploadFileDto
    {
        public IFormFile File { get; set; }
    }
}
