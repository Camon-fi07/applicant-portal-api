using Common.Constants;
using Common.Dto;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Notification.BL.Services;
using Notification.BL.Services.IServices;

namespace Notification.BL.Extensions
{
    public static class ConfigureNotification
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ISendService, SendService>();
        }

        public static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var sendService = serviceProvider.GetRequiredService<ISendService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Subscribe<EmailMessageDto>(RabbitMQQueue.NOTIFICATION_SEND_EMAIL, (data) => sendService.SendEmail(data));
        }
    }
}
