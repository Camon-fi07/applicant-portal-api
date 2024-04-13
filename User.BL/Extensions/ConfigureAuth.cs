using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using User.DAL;

namespace User.BL.Extensions
{
    public static class ConfigureAuth
    {
        public static void AddUserDB(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<UserDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
