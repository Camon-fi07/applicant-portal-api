using Admission.BL.Services.IServices;
using Admission.DAL;
using Common.Constants;
using Common.Dto.Application;
using Common.Enums;
using Common.Models;
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

        public static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var synchronizationService = serviceProvider.GetRequiredService<ISynchronizationService>();
            var applicationService = serviceProvider.GetRequiredService<IApplicationService>();
            var managerService = serviceProvider.GetRequiredService<IManagerService>();

            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Subscribe<GuideType>(RabbitMQQueue.REFRESH_GUIDE, async (type) => await synchronizationService.SychronizeGuides(type), x => x.WithQueueName(RabbitMQQueue.REFRESH_GUIDE));
            bus.Rpc.Respond<ApplicationFilterModel, ApplicationsPagedListModel>(async (f) => await applicationService.GetApplications(f.Page, f.Size, f.Name, f.ProgramId, f.Faculties, f.Status, f.HaveManager, f.ManagerId, f.SortType),
            x => x.WithQueueName(RabbitMQQueue.GET_APPLICATIONS));

            bus.Rpc.Respond<ManagerQueryModel, List<UserDto>>((query) => managerService.GetManagers(query.Email), x => x.WithQueueName(RabbitMQQueue.GET_MANAGERS));
            bus.Rpc.Respond<Guid, ApplicationDto>(async id => await applicationService.GetApplication(id), x => x.WithQueueName(RabbitMQQueue.GET_APPLICATION));
        }
    }
}
