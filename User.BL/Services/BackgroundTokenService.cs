using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using User.DAL;

namespace User.BL.Services
{
    public class BackgroundTokenService:BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public BackgroundTokenService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<UserDBContext>();
                    var refrehTokens = dbContext.RefreshTokens;
                    foreach (var token in refrehTokens)
                    {
                        if(token.ExpiredTime < DateTime.UtcNow)
                        {
                            dbContext.RefreshTokens.Remove(token);
                        }
                    }
                    await dbContext.SaveChangesAsync();
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
