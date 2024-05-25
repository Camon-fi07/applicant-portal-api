using AdminPanel.Models;
using Common.Dto.Application;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IAdminPanelApplicationService _adminPanelApplicationService;
        private readonly IAdminPanelDictionaryService _adminPanelDictionaryService;
        public ApplicationController(IAdminPanelApplicationService adminPanelApplicationService, IAdminPanelDictionaryService adminPanelDictionaryServiced) { 
            _adminPanelApplicationService = adminPanelApplicationService;
            _adminPanelDictionaryService = adminPanelDictionaryServiced;
        }

        [Route("id:guid")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(Guid id)
        {
            var application = await _adminPanelApplicationService.GetApplication(id);

            var model = new ApplicationViewModel()
            {
                Application = application
            };
            
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List(ApplicationFilterModel filterModel)
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            var applications = await _adminPanelApplicationService.GetApplications(filterModel);

            var faculties = await _adminPanelDictionaryService.GetFaculties();
            var programs = await _adminPanelDictionaryService.GetPrograms();
            var managers = await _adminPanelApplicationService.GetManagers(new ManagerQueryModel());

            var model = new ApplicationListViewModel()
            {
                ApplicationsPagedListModel = applications,
                Filter = filterModel,
                AvailableFaculties = faculties,
                AvailablePrograms = programs,
                AvailableManagers = managers
            };

            return View(model);
        }
    }
}
