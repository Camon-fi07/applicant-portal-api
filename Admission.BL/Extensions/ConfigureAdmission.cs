using Admission.BL.Dto;
using Admission.BL.Services.IServices;
using Admission.DAL;
using Common.Constants;
using Common.Dto;
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
            var admissionService = serviceProvider.GetRequiredService<IManagerService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            await bus.PubSub.SubscribeAsync<AddUserDto>(RabbitMQQueue.ADMISSION_ADD_USER, data => admissionService.AddUser(data));
            await bus.PubSub.SubscribeAsync<EditUserDto>(RabbitMQQueue.ADMISSION_EDIT_USER, data => admissionService.EditUser(data));
        }
    }
}
