using AdminPanel.BL.Services;
using AdminPanel.Models;
using Common.Dto.Applicant;
using Common.Dto.User;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    [Authorize(Roles =$"{nameof(RoleType.Administrator)},{nameof(RoleType.GeneralManager)},{nameof(RoleType.Manager)}")]
    public class ApplicantController : Controller
    {
        private readonly IAdminPanelProfileService _adminPanelProfileService;
        private readonly IAdminPanelApplicantService _adminPanelApplicantService;
        public ApplicantController(IAdminPanelProfileService adminPanelProfileService, IAdminPanelApplicantService adminPanelApplicantService) {
            _adminPanelProfileService = adminPanelProfileService;
            _adminPanelApplicantService = adminPanelApplicantService;
        }

        [HttpGet("Applicant/Index/{applicantId:guid}")]
        public async Task<IActionResult> Index(Guid applicantId)
        {
            var profile = await _adminPanelProfileService.GetProfile(applicantId);
            var applicantData = await _adminPanelApplicantService.GetApplicantData(applicantId);
            var model = new ApplicantViewModel()
            {
                ApplicantData = applicantData,
                EditApplicantData = new EditApplicantDataDto(),
                EditProfile = new EditProfileDto(),
                Profile = profile
            };

            return View(model);
        }

        [HttpPut("Applicant/EditProfile/{applicantId:guid}")]
        public async Task<IActionResult> EditProfile(Guid applicantId, [FromForm] EditProfileDto editProfile)
        {
            var profile = await _adminPanelProfileService.EditProfile(applicantId, editProfile);
            var applicantData = await _adminPanelApplicantService.GetApplicantData(applicantId);

            var model = new ApplicantViewModel()
            {
                ApplicantData = applicantData,
                EditApplicantData = new EditApplicantDataDto(),
                EditProfile = editProfile,
                Profile = profile
            };

            return View("Index", model);
        }

        [HttpPut("Applicant/EditApplicant/{applicantId:guid}")]
        public async Task<IActionResult> EditApplicant(Guid applicantId, [FromForm] EditApplicantDataDto editApplicantData)
        {
            var applicantData = await _adminPanelApplicantService.EditApplicantData(editApplicantData, applicantId);

            var profile = await _adminPanelProfileService.GetProfile(applicantId);

            var model = new ApplicantViewModel()
            {
                ApplicantData = applicantData,
                EditApplicantData = editApplicantData,
                EditProfile = new EditProfileDto(),
                Profile = profile
            };

            return View("Index", model);
        }
    }
}
