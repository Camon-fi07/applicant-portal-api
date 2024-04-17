using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using User.DAL;
using User.DAL.Entities;

namespace User.BL.Extensions
{
    public class TokenBlackListHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;

        public TokenBlackListHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager,
        IServiceProvider serviceProvider) : base(options, logger, encoder, clock)
        {
            _serviceProvider = serviceProvider;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userDbContext = _serviceProvider.GetRequiredService<UserDBContext>();

            string token = Request.Headers["Authorization"].FirstOrDefault();

            var bannedToken = userDbContext.BannedTokens.FirstOrDefault(t => t.Token == token);

            if (bannedToken != null) return AuthenticateResult.Fail("Токен забанен");


            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

            if(userId == null) return AuthenticateResult.Fail("Пользователь не найден");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return AuthenticateResult.Fail("Пользователь не найден");

            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
