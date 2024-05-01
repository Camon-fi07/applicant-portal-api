using Applicant.BL.Dto;
using Applicant.BL.Services;
using Applicant.BL.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Applicant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantController : ControllerBase
    {
        private readonly IApplicantService _applicantService;
        public ApplicantController(IApplicantService applicantService) {
            _applicantService = applicantService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterApplicant([FromForm] RegisterApplicantDto registerApplicantDto)
        {
            await _applicantService.RegisterApplicant(registerApplicantDto);
            return Ok();
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApplicantProfileDto>> GetApplicantProfile()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var profile = await _applicantService.GetProfile(userId);
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApplicantProfileDto>> EditApplicantProfile([FromBody] EditApplicantProfileDto editApplicantProfileDto)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var profile = await _applicantService.EditProfile(editApplicantProfileDto, userId);
            return Ok(profile);
        }
    }
}
