using Applicant.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Applicant.BL.Extensions
{
    public static class ConfigureApplicant
    {
        public static void AddApplicantDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicantDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
