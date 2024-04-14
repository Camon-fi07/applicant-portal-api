using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using User.BL.Models;
using User.DAL;

namespace User.BL.Services
{
    public class BackgroundTokenService:BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private TokenProperties _tokenProperties;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        public BackgroundTokenService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _tokenProperties = new TokenProperties();
            configuration.GetSection("TokenProperties:accessToken").Bind(_tokenProperties);
            _serviceProvider = serviceProvider;
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenProperties.secret)),
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateAudience = false
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<UserDBContext>();
                    var bannedTokens = dbContext.BannedTokens;
                    foreach (var token in bannedTokens)
                    {
                        try
                        {
                            _tokenHandler.ValidateToken(token.Token, _tokenValidationParameters, out _);
                        }
                        catch
                        {
                            dbContext.BannedTokens.Remove(token);
                        }

                    }
                    await dbContext.SaveChangesAsync();
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
