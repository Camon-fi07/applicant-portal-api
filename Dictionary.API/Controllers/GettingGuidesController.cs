using Common.Dto;
using Common.Dto.Dictionary;
using Dictionary.BL.Dto;
using Dictionary.BL.Models;
using Dictionary.BL.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dictionary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GettingGuidesController : ControllerBase
    {
        IGettingService _gettingService;
        public GettingGuidesController(IGettingService gettingService)
        {
            _gettingService = gettingService;
        }

        [HttpGet("education_levels")]
        public async Task<ActionResult<List<EducationLevelDto>>> GetEducationLevels(string? name)
        {
            var educationLevels = await _gettingService.GetEducationLevels(name);

            return Ok(educationLevels);
        }

        [HttpGet("document_types")]
        public async Task<ActionResult<List<EducationDocumentTypeDto>>> GettDocumentTypes(string? name, int? educationLevelId)
        {
            var documentTypes = await _gettingService.GetDocumentTypes(name, educationLevelId);

            return Ok(documentTypes);
        }

        [HttpGet("faculties")]
        public async Task<ActionResult<List<EducationFacultyDto>>> GetFaculties(string? name)
        {
            var faculties = await _gettingService.GetFaculties(name);

            return Ok(faculties);
        }

        [HttpGet("programs")]
        public async Task<ActionResult<ProgramPagedListModel>> GetPrograms(Guid? facultyId, int? educationLevelId, string? educationForm, string? language, string? name, string? code, int page = 1, int size = 10)
        {
            var programs = await _gettingService.GetPrograms(page, size, facultyId, educationLevelId, educationForm, language, name, code);

            return Ok(programs);
        }


        [HttpGet("importing_statuses")]
        public async Task<ActionResult<ImportingStatusDto>> GetImportingStatuses()
        {
            var statuses = await _gettingService.GetImportingStatuses();

            return Ok(statuses);
        }
    }
}
