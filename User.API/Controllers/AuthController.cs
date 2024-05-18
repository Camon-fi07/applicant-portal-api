using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.BL.Services.IServices;
using Common.Dto;
using User.BL.Dto;
using Common.Dto.User;
using Common.Models;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        public AuthController(IAuthService authService, ITokenService tokenService) {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.Login(loginDto, HttpContext.Request.Headers.UserAgent.ToString());

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> RegisterApplicant([FromBody] UserRegisterModel userRegisterModel)
        {
            var token = await _authService.RegisterUser(userRegisterModel, HttpContext.Request.Headers.UserAgent.ToString());

            return Ok(token);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<TokenResponseDto>> Logout()
        {
            if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
            {
                return Unauthorized();
            }

            await _authService.Logout(userId, HttpContext.Request.Headers.UserAgent.ToString());

            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {

            var token = await _tokenService.RefreshToken(refreshTokenDto, HttpContext.Request.Headers.UserAgent.ToString());

            return Ok(token);
        }
    }
}
