using Microsoft.Extensions.DependencyInjection;
using User.BL.Services;
using User.BL.Services.IServices;

namespace User.BL.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureUserServices(this IServiceCollection services) {

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddHostedService<BackgroundTokenService>();
        }
    }
}
