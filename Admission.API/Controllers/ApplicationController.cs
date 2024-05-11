using Admission.BL.Dto;
using Admission.BL.Enums;
using Admission.BL.Models;
using Admission.BL.Services.IServices;
using Common.Enums;
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

        [HttpGet("all")]
        [Authorize]
        public async Task<ActionResult<ApplicationsPagedListModel>> GetApplications(string? name, Guid? programId, List<Guid>? faculties, ApplicationStatus? status, bool? haveManager, Guid? managerId, ApplicationsSortType? sortType, int page = 1, int size=20)
        {
            var applications = await _applicationService.GetApplications(page, size, name, programId, faculties, status, haveManager, managerId, sortType);

            return Ok(applications);
        }

        [HttpPut("{id:guid}/programs")]
        [Authorize]
        public async Task<IActionResult> EditPrograms(Guid id, [FromBody] List<ProgramPriorityDto> programPriorities)
        {
            await _applicationService.EditPrograms(programPriorities, id);
            return Ok();
        }
    }
}
