using AdminPanel.Models;
using Common.Dto.User;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAdminPanelProfileService _profileService;
        public UsersController(IAdminPanelProfileService profileService)
        {
            _profileService = profileService;
        }

        public IActionResult Index()
        {
            var model = new UsersListViewModel()
            {
                Users = new List<ProfileWithRolesDto>()
            };

            return View(model);
        }

        [HttpDelete("User/{id:guid}/Role")]
        public async Task<IActionResult> DeleteManager(Guid id)
        {
            await _profileService.DeleteManagerRole(id);

            var model = new UsersListViewModel()
            {
                Users = new List<ProfileWithRolesDto>()
            };

            return View("Index", model);
        }

        [HttpPut("User/{id:guid}/Role")]
        public async Task<IActionResult> EditManager(Guid id, [FromBody] ManagerRole role)
        {
            await _profileService.SetManagerRole(id, role);

            var model = new UsersListViewModel()
            {
                Users = new List<ProfileWithRolesDto>()
            };

            return View("Index", model);
        }
    }
}
