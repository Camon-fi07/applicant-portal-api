using Common.Dto;
using Common.Enums;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = nameof(RoleType.Administrator))]
        [HttpPost("education_levels")]
        public async Task<IActionResult> ImportEducationLevels() {
            await _importingService.ImportEducationLevels();

            return Ok();
        }

        [Authorize(Roles = nameof(RoleType.Administrator))]
        [HttpPost("document_types")]
        public async Task<IActionResult> ImportDocumentTypes()
        {
            await _importingService.ImporDocumentTypes();

            return Ok();
        }

        [Authorize(Roles = nameof(RoleType.Administrator))]
        [HttpPost("faculties")]
        public async Task<IActionResult> ImportFaculties()
        {
            await _importingService.ImportFaculties();

            return Ok();
        }

        [Authorize(Roles = nameof(RoleType.Administrator))]
        [HttpPost("programs")]
        public async Task<IActionResult> ImportPrograms()
        {
            await _importingService.ImportPrograms();

            return Ok();
        }
    }
}
