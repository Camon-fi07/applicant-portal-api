using Common.Dto.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("roles")]
        [Authorize(Policy = "TokenNotInBlackList")]
        public async Task<ActionResult<ProfileDto>> GetUserRoles()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            var roles = await _profileService.GetUserRoles(token);
            return Ok(roles);
        }
    }
}
