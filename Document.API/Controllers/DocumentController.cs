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

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileDto uploadFileDto)
        {
            await _documentService.UploadFile(uploadFileDto);

            return Ok();
        }

        [HttpPost("uploadFiles")]
        public async Task<IActionResult> UploadFiles([FromForm] List<UploadFileDto> uploadFilesDto)
        {
            await _documentService.UploadFiles(uploadFilesDto);

            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FileResponseDto>> GetFile(Guid id)
        {
            var file = await _documentService.GetFile(id);

            return Ok(file);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            await _documentService.DeleteFile(id);

            return Ok();
        }
    }
}
