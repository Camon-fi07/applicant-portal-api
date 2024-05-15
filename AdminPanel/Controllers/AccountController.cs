using Common.Dto.User;
using Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace AdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminPanelAccountService _adminPanelAccountService;
        public AccountController(IAdminPanelAccountService adminPanelAccountService) { 
            _adminPanelAccountService = adminPanelAccountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var tokens = await _adminPanelAccountService.Login(loginDto);
            Response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions { HttpOnly = true });
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions { HttpOnly = true });

            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            Console.WriteLine(User.Identity.Name);
            return View();
        }
    }
}
