using AdminPanel.Models;
using Common.Dto.User;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.GeneralManager)},{nameof(RoleType.Manager)}")]
    public class ProfileController : Controller
    {
        private readonly IAdminPanelProfileService _adminPanelProfileService;
        public ProfileController(IAdminPanelProfileService adminPanelProfileService) {
            _adminPanelProfileService = adminPanelProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var profile = await _adminPanelProfileService.GetProfile(userId);

            var model = new EditProfileViewModel()
            {
                BirthDate = profile.BirthDate,
                Email = profile.Email,
                Firstname = profile.FirstName,
                Gender = profile.Gender,
                Lastname = profile.LastName,
                Middlename = profile.MiddleName,
                PhoneNumber = profile.PhoneNumber,
            };
            
            return View(model);
        }

        [HttpPut]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editProfileViewModel)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var profile = await _adminPanelProfileService.EditProfile(userId, new EditProfileDto()
            {
                BirthDate = editProfileViewModel.BirthDate,
                Email= editProfileViewModel.Email,
                Firstname = editProfileViewModel.Firstname,
                Gender = editProfileViewModel.Gender,
                Lastname= editProfileViewModel.Lastname,
                Middlename = editProfileViewModel.Middlename,
                PhoneNumber = editProfileViewModel.PhoneNumber
            });

            var model = new EditProfileViewModel()
            {
                BirthDate = profile.BirthDate,
                Email = profile.Email,
                Firstname = profile.FirstName,
                Gender = profile.Gender,
                Lastname = profile.LastName,
                Middlename = profile.MiddleName,
                PhoneNumber = profile.PhoneNumber,
            };

            return View(model);
        }
    }
}
