using Dictionary.BL.Services;
using Dictionary.BL.Services.IServices;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;

namespace Dictionary.BL.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureDictionaryServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            var bus = RabbitHutch.CreateBus("host=localhost");
            services.AddSingleton(bus);
            services.AddScoped<IImportingService, ImportingService>();
            services.AddScoped<IGettingService, GettingService>();
        }
    }
}
