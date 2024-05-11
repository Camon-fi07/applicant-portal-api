using Common.Constants;
using Common.Dto.Applicant;
using Common.Dto.RabbitMQ;
using Dictionary.BL.Dto;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL;
using EasyNetQ;
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

        public async static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var gettingService = serviceProvider.GetRequiredService<IGettingService>();
            var bus = RabbitHutch.CreateBus("host=localhost");
        }
    }
}
