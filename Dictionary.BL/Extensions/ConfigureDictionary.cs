using Dictionary.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dictionary.BL.Extensions
{
    public static class ConfigureDictionary
    {
        public static void AddDictionaryDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DictionaryDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
