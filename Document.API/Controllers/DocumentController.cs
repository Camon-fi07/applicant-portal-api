using Common.Dto;
using Document.BL.Dto;
using Document.BL.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Document.API.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService) {
           _documentService = documentService;
        }

        [Authorize]
        [HttpPost("uploadFile")]
        public async Task<ActionResult<FileDto>> UploadFile([FromForm] UploadFileDto uploadFileDto)
        {
            var result = await _documentService.UploadFile(uploadFileDto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FileDto>> GetFile(Guid id)
        {
            var file = await _documentService.GetFile(id);

            return Ok(file);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            await _documentService.DeleteFile(id);

            return Ok();
        }
    }
}
