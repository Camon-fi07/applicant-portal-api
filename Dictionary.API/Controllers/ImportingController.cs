using Dictionary.BL.Services.IServices;
using Dictionary.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dictionary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportingController : ControllerBase
    {
        IImportingService _importingService;
        public ImportingController(IImportingService importingService) {
            _importingService = importingService;
        }

        [HttpPost("education_levels")]
        public async Task<IActionResult> ImportEducationLevels() {
            await _importingService.ImportEducationLevels();

            return Ok();
        }

        [HttpPost("document_types")]
        public async Task<IActionResult> ImportDocumentTypes()
        {
            await _importingService.ImporDocumentTypes();

            return Ok();
        }

        [HttpPost("faculties")]
        public async Task<IActionResult> ImportFaculties()
        {
            await _importingService.ImportFaculties();

            return Ok();
        }

        [HttpPost("programs")]
        public async Task<IActionResult> ImportPrograms()
        {
            await _importingService.ImportPrograms();

            return Ok();
        }

        [HttpGet("importing_statuses")]
        public async Task<ActionResult<ImportingStatusEntity>> GetImportingStatuses()
        {
            var statuses = await _importingService.GetImportingStatuses();

            return Ok(statuses);
        }
    }
}
