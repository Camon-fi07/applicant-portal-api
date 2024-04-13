using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.BL.Services;
using User.BL.Services.IServices;

namespace User.BL.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureUserServices(this IServiceCollection services) {

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
