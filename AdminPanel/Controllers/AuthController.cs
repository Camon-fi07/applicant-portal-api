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
    public class AuthController : Controller
    {
        private readonly IAdminPanelAuthService _adminPanelAuthService;

        public AuthController(IAdminPanelAuthService adminPanelAuthService) {
            _adminPanelAuthService = adminPanelAuthService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var tokens = await _adminPanelAuthService.Login(loginDto, HttpContext.Request.Headers.UserAgent.ToString());
            Response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions { HttpOnly = true });
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions { HttpOnly = true });

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            await _adminPanelAuthService.Logout(userId, HttpContext.Request.Headers.UserAgent.ToString());

            return Ok();
        }
    }
}
