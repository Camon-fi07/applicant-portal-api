using AdminPanel.Models;
using Common.Dto.User;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.GeneralManager)},{nameof(RoleType.Manager)}")]
    public class UsersController : Controller
    {
        private readonly IAdminPanelProfileService _profileService;
        public UsersController(IAdminPanelProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index(string? email)
        {
            var users = await _profileService.GetAllUsers(email);
            var model = new UsersListViewModel()
            {
                Users = users,
                EmailQuery = email ?? "",
            };

            return View(model);
        }

        [HttpDelete("User/{id:guid}/Role")]
        public async Task<IActionResult> DeleteManager(Guid id)
        {
            await _profileService.DeleteManagerRole(id);

            return Ok();
        }

        [HttpPut("User/{id:guid}/Role")]
        public async Task<IActionResult> SetManager(Guid id, [FromBody] ManagerRole role)
        {
            await _profileService.SetManagerRole(id, role);

            return Ok();
        }
    }
}
