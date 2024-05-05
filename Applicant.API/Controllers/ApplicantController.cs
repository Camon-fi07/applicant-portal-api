using Applicant.BL.Dto;
using Applicant.BL.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Dto.Applicant;

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
        public async Task<IActionResult> RegisterApplicant([FromBody] RegisterApplicantDto registerApplicantDto)
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

        [Authorize]
        [HttpGet("passport")]
        public async Task<ActionResult<PassportDto>> GetApplicantPassport()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var passport = await _applicantService.GetPassport(userId);
            return Ok(passport);
        }

        [Authorize]
        [HttpPut("passport")]
        public async Task<ActionResult<PassportDto>> EditApplicantPassport([FromBody] EditPassportDto editPassportDto)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var passport = await _applicantService.EditPassport(editPassportDto, userId);
            return Ok(passport);
        }

        [Authorize]
        [HttpGet("education")]
        public async Task<ActionResult<EducationDocumentDto>> GetApplicantEducation()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var educationDocument = await _applicantService.GetEducationDocument(userId);
            return Ok(educationDocument);
        }

        [Authorize]
        [HttpPut("education")]
        public async Task<ActionResult<EducationDocumentDto>> EditApplicantEducation([FromBody] EditEducationDocumentDto editEducationDocumentDto)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var educationDocument = await _applicantService.EditEducationDocument(editEducationDocumentDto, userId);
            return Ok(educationDocument);
        }
    }
}
