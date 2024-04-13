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
        public async Task<ActionResult<TokenResponseDto>> register([FromBody] RegisterApplicantDto data)
        {
            var token = await _authService.RegisterApplicant(data);

            return Ok(token);
        }
    }
}
