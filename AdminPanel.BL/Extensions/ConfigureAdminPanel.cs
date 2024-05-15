using AdminPanel.BL.Services;
using Common.Constants;
using Common.Dto.RabbitMQ;
using Common.Dto;
using Common.Interfaces;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Common.Dto.User;

namespace AdminPanel.BL.Extensions
{
    public static class ConfigureAdminPanel
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminPanelAccountService, AdminPanelAccountService>();
            var bus = RabbitHutch.CreateBus("host=localhost");
            services.AddSingleton(bus);
        }

        public static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var adminPanelAccountService = serviceProvider.GetRequiredService<IAdminPanelAccountService>();
            var bus = RabbitHutch.CreateBus("host=localhost");
        }
    }
}
