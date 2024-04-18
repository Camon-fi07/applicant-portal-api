using Common.Dto;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.BL.Dto;
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
        [Authorize]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            var user = await _profileService.GetProfile(token);
            return Ok(user);
        }

        [HttpGet("roles")]
        [Authorize(AuthenticationSchemes = "TokenBlackList")]
        public async Task<ActionResult<List<RoleType>>> GetUserRoles()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            var roles = await _profileService.GetUserRoles(token);
            return Ok(roles);
        }

        [HttpPost("passwordReset")]
        [Authorize(AuthenticationSchemes = "TokenBlackList")]
        public async Task<ActionResult<List<RoleType>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            await _profileService.ResetPassword(token, resetPasswordDto);
            return Ok();
        }
    }
}
