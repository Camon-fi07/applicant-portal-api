using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.BL.Dto.Profile;
using User.BL.Services.IServices;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("")]
        [Authorize(Policy = "TokenNotInBlackList")]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            var user = await _profileService.GetProfile(token);
            return Ok(user);
        }

        [HttpPut("applicant")]
        [Authorize(Policy = "TokenNotInBlackList", Roles = "Applicant")]
        public async Task<IActionResult> EditApplicantProfile([FromBody] EditApplicantProfileDto editApplicantProfileDto)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            await _profileService.EditApplicantProfile(token, editApplicantProfileDto);
            return Ok();
        }

        [HttpPut("manager")]
        [Authorize(Policy = "TokenNotInBlackList", Roles = "Manager")]
        public async Task<IActionResult> EditManagerProfile([FromBody] EditManagerDto editManagerDto)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            await _profileService.EditManagerProfile(token, editManagerDto);
            return Ok();
        }

        [HttpGet("roles")]
        [Authorize(Policy = "TokenNotInBlackList")]
        public async Task<ActionResult<List<RoleType>>> GetUserRoles()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            var roles = await _profileService.GetUserRoles(token);
            return Ok(roles);
        }
    }
}
