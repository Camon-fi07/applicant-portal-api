using Admission.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.BL.Extensions
{
    public static class ConfigureAdmission
    {
        public static void AddAdmissionDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AdmissionDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
