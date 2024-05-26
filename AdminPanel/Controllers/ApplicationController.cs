using AdminPanel.Models;
using Common.Dto.Application;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.GeneralManager)},{nameof(RoleType.Manager)}")]
    public class ApplicationController : Controller
    {
        private readonly IAdminPanelApplicationService _adminPanelApplicationService;
        private readonly IAdminPanelDictionaryService _adminPanelDictionaryService;
        private readonly int maxProgramsCount;
        public ApplicationController(IAdminPanelApplicationService adminPanelApplicationService, IAdminPanelDictionaryService adminPanelDictionaryServiced, IConfiguration configuration) { 
            _adminPanelApplicationService = adminPanelApplicationService;
            _adminPanelDictionaryService = adminPanelDictionaryServiced;

            if (!int.TryParse(configuration.GetSection("MaxProgramsCount").Value, out maxProgramsCount))
            {
                maxProgramsCount = 5;
            }
        }

        [HttpGet("Application/{id:guid}")]
        public async Task<IActionResult> Index(Guid id)
        {
            var application = await _adminPanelApplicationService.GetApplication(id);
            var managers = await _adminPanelApplicationService.GetManagers(new ManagerQueryModel());

            var model = new ApplicationViewModel()
            {
                Application = application,
                MaxProgramsCount = maxProgramsCount,
                AvailableManagers = managers
            };
            
            return View(model);
        }
 
        [HttpPut]
        public async Task<IActionResult> EditPrograms(Guid id, [FromBody] List<CreateProgramPriorityDto> programs)
        {
            await _adminPanelApplicationService.EditPrograms(programs, id);

            var application = await _adminPanelApplicationService.GetApplication(id);
            var managers = await _adminPanelApplicationService.GetManagers(new ManagerQueryModel());

            var model = new ApplicationViewModel()
            {
                Application = application,
                MaxProgramsCount = maxProgramsCount,
                AvailableManagers = managers
            };

            return View("Index", model);
        }

        [HttpPut]
        public async Task<IActionResult> ReturnApplication(Guid id)
        {
            await _adminPanelApplicationService.ReturnApplication(id);

            var application = await _adminPanelApplicationService.GetApplication(id);
            var managers = await _adminPanelApplicationService.GetManagers(new ManagerQueryModel());

            var model = new ApplicationViewModel()
            {
                Application = application,
                MaxProgramsCount = maxProgramsCount,
                AvailableManagers = managers
            };

            return View("Index", model);
        }

        [HttpPut("Application/{applicationId:guid}/SetManager/{managerId:guid}")]
        public async Task<IActionResult> SetManager(Guid applicationId, Guid managerId)
        {
            await _adminPanelApplicationService.SetManager(managerId, applicationId);

            var application = await _adminPanelApplicationService.GetApplication(applicationId);
            var managers = await _adminPanelApplicationService.GetManagers(new ManagerQueryModel());

            var model = new ApplicationViewModel()
            {
                Application = application,
                MaxProgramsCount = maxProgramsCount,
                AvailableManagers = managers
            };

            return View("Index", model);
        }

        [HttpPut]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ApplicationStatus status)
        {
            await _adminPanelApplicationService.ChangeApplicationStatus(id, status);

            var application = await _adminPanelApplicationService.GetApplication(id);
            var managers = await _adminPanelApplicationService.GetManagers(new ManagerQueryModel());

            var model = new ApplicationViewModel()
            {
                Application = application,
                MaxProgramsCount = maxProgramsCount,
                AvailableManagers = managers
            };

            return View("Index", model);
        }

        [HttpGet]
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
