using Common.Dto;
using Document.BL.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Document.API.Controllers
{   
    public class Test
    {
        public IFormFile file { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService) {
           _documentService = documentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile formFile)
        {
            await _documentService.UploadFile(formFile);

            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FileResponseDto>> GetFile(Guid id)
        {
            var file = await _documentService.GetFile(id);

            return Ok(file);
        }

        [HttpPut("change/{id:guid}")]
        public async Task<IActionResult> ChangeFile(IFormFile formFile, Guid id)
        {
            await _documentService.ChangeFile(formFile, id);

            return Ok();
        }
    }
}
