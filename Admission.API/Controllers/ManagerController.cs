using Admission.BL.Services.IServices;
using Common.Dto.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto createManagerDto)
        {
            await _managerService.CreateManager(createManagerDto);
            return Ok();
        }

        [HttpDelete("{managerId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteManager(Guid managerId)
        {
            await _managerService.DeleteManager(managerId);
            return Ok();
        }

        [HttpPost("{managerId:guid}/addToFaculty/{facultyId:Guid}")]
        [Authorize]
        public async Task<IActionResult> AddManagerToFaculty(Guid managerId, Guid facultyId)
        {
            await _managerService.AddManagerToFaculty(managerId, facultyId);
            return Ok();
        }

        [HttpDelete("{managerId:guid}/deleteFromFaculty")]
        [Authorize]
        public async Task<IActionResult> DeleteManagerFromFaculty(Guid managerId)
        {
            await _managerService.DeleteManagerFromFaculty(managerId);
            return Ok();
        }
    }
}
