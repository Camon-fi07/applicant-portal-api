using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.BL.Services.IServices;
using Common.Dto;
using Common.RabbitMQ;
using User.BL.Dto;
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
            var token = await _authService.Login(loginDto);

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> RegisterApplicant([FromBody] UserRegisterModel userRegisterModel)
        {
            var token = await _authService.RegisterUser(userRegisterModel);

            return Ok(token);
        }

        [Authorize(AuthenticationSchemes = "TokenBlackList")]
        [HttpPost("logout")]
        public async Task<ActionResult<TokenResponseDto>> Logout()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            await _authService.Logout(token);

            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var token = await _tokenService.RefreshToken(refreshTokenDto);

            return Ok(token);
        }
    }
}
