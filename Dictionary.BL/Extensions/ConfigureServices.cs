using Dictionary.BL.Services;
using Dictionary.BL.Services.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace Dictionary.BL.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureDictionaryServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IImportingService, ImportingService>();
        }
    }
}
