using Common.Dto.Auth;
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

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterApplicantDto data)
        {
            var token = await _authService.RegisterApplicant(data);

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto data)
        {
            var token = await _authService.Login(data);

            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto data)
        {
            var token = await _tokenService.RefreshToken(data);
            return Ok(token);
        }
    }
}
