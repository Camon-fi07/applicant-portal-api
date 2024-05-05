using Admission.BL.Dto;
using Admission.BL.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService) { 
            _applicationService = applicationService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto createApplication)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            await _applicationService.CreateApplication(createApplication, userId);
            return Ok();
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApplicationDto>> GetApplication(Guid id)
        {
            return Ok(await _applicationService.GetApplication(id));
        }

        [HttpPut("{id:guid}/programs")]
        [Authorize]
        public async Task<IActionResult> EditPrograms(Guid id, [FromBody] List<ProgramPriorityDto> programPriorities)
        {
            await _applicationService.EditProgramPriorities(programPriorities, id);
            return Ok();
        }
    }
}
