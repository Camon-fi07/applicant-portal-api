using Applicant.BL.Dto;
using Applicant.BL.Services;
using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Applicant.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicantController : ControllerBase
    {
        private readonly ApplicantService _applicantService;
        public ApplicantController(ApplicantService applicantService) {
            _applicantService = applicantService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterApplicant([FromBody] RegisterApplicantDto registerApplicantDto)
        {
            await _applicantService.RegisterApplicant(registerApplicantDto);
            return Ok();
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApplicantProfileDto>> GetApplicantProfile()
        {
            //var profile = await _applicantService.GetProfile();
            return Ok();
        }

        [HttpPut("profile")]
        public async Task<IActionResult> EditApplicantProfile([FromBody] EditApplicantProfileDto editApplicantProfileDto)
        {
            //await _applicantService.EditApplicantProfile(editApplicantProfileDto);
            return Ok();
        }
    }
}
