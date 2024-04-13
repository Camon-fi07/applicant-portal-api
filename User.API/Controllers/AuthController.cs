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
        public AuthController(IAuthService authService) {
            _authService = authService;
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
    }
}
