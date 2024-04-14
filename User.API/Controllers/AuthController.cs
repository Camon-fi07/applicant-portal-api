using User.BL.Dto.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.BL.Services.IServices;

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

        [HttpPost("register/applicant")]
        public async Task<ActionResult<TokenResponseDto>> RegisterApplicant([FromBody] RegisterApplicantDto registerApplicantDto)
        {
            var token = await _authService.RegisterApplicant(registerApplicantDto);

            return Ok(token);
        }

        [HttpPost("register/manager")]
        public async Task<ActionResult<TokenResponseDto>> RegisterManager([FromBody] RegisterManagerDto registerManagerDto)
        {
            var token = await _authService.RegisterManager(registerManagerDto);

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.Login(loginDto);

            return Ok(token);
        }

        [Authorize(Policy = "TokenNotInBlackList")]
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
