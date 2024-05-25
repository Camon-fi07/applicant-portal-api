using Admission.BL.Services.IServices;
using Admission.DAL;
using Common.Constants;
using Common.Enums;
using EasyNetQ;
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

        public async static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var synchronizationService = serviceProvider.GetRequiredService<ISynchronizationService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Subscribe<GuideType>(RabbitMQQueue.REFRESH_GUIDE, async (type) => await synchronizationService.SychronizeGuides(type), x => x.WithQueueName(RabbitMQQueue.REFRESH_GUIDE));
        }
    }
}
