using AdminPanel.Models;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = nameof(RoleType.Administrator))]
    public class DictionaryController : Controller
    {
        private readonly IAdminPanelDictionaryService _adminPanelDictionaryService;
        public DictionaryController(IAdminPanelDictionaryService adminPanelDictionaryService) {
            _adminPanelDictionaryService = adminPanelDictionaryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var statuses = await _adminPanelDictionaryService.GetImportingStatuses();
            return View(new DictionaryViewModel()
            {
                ImportingStatuses = statuses
            });
        }

        [HttpPost]
        public async Task<IActionResult> ImportPrograms()
        {
            await _adminPanelDictionaryService.ImportPrograms();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ImportEducationLevels()
        {
            await _adminPanelDictionaryService.ImportEducationLevels();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ImportEducationDocuments()
        {
            await _adminPanelDictionaryService.ImportEducationDocuments();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ImportFaculties()
        {
            await _adminPanelDictionaryService.ImportFaculties();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetImportingStatuses()
        {
            var statuses = await _adminPanelDictionaryService.GetImportingStatuses();
            return View(new DictionaryViewModel()
            {
                ImportingStatuses = statuses
            });
        }
    }
}
