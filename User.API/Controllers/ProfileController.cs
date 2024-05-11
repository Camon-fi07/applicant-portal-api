using Common.Dto;
using Common.Dto.Applicant;
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
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var user = await _profileService.GetProfile(userId);

            return Ok(user);
        }

        [HttpPut("")]
        [Authorize]
        public async Task<ActionResult<ProfileDto>> EditProfile(EditProfileDto editProfileDto)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }
            var profile = await _profileService.EditProfile(userId,editProfileDto);

            return Ok(profile);
        }

        [HttpGet("applicant")]
        [Authorize]
        public async Task<ActionResult<ApplicantProfileDto>> GetApplicantProfile()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var user = await _profileService.GetApplicantProfile(userId);

            return Ok(user);
        }

        [HttpPut("applicant")]
        [Authorize]
        public async Task<ActionResult<ApplicantProfileDto>> EditApplicantProfile(EditApplicantProfileDto editApplicantProfileDto)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }
            var profile = await _profileService.EditApplicantProfile(userId, editApplicantProfileDto);

            return Ok(profile);
        }

        [HttpGet("roles")]
        [Authorize]
        public async Task<ActionResult<List<RoleType>>> GetUserRoles()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var roles = await _profileService.GetUserRoles(userId);

            return Ok(roles);
        }

        [HttpPost("passwordReset")]
        [Authorize]
        public async Task<ActionResult<List<RoleType>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            await _profileService.ResetPassword(token, resetPasswordDto);

            return Ok();
        }
    }
}
