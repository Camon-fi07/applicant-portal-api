using Applicant.BL.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Dto.Applicant;
using Common.Enums;

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

        [Authorize(Roles = nameof(RoleType.Applicant))]
        [HttpGet("profile")]
        public async Task<ActionResult<ApplicantDataDto>> GetApplicantData()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var profile = await _applicantService.GetApplicantData(userId);
            return Ok(profile);
        }

        [Authorize(Roles = nameof(RoleType.Applicant))]
        [HttpPut("profile")]
        public async Task<ActionResult<ApplicantDataDto>> EditApplicantProfile([FromBody] EditApplicantDataDto editApplicantDataDto)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var profile = await _applicantService.EditApplicantData(editApplicantDataDto, userId);
            return Ok(profile);
        }

        [Authorize(Roles = nameof(RoleType.Applicant))]
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

        [Authorize(Roles = nameof(RoleType.Applicant))]
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

        [Authorize(Roles = nameof(RoleType.Applicant))]
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

        [Authorize(Roles = nameof(RoleType.Applicant))]
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
