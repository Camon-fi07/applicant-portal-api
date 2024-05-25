using Admission.BL.Services.IServices;
using Admission.DAL;
using Common.Constants;
using Common.Dto;
using Common.Dto.Application;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using Common.Enums;
using Common.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            bus.PubSub.Subscribe<AddUserDto>(RabbitMQQueue.SYNCHRONIZE_PROFILE, async (data) => { await synchronizationService.SychronizeUser(data); }, x => x.WithTopic(RabbitMQQueue.SYNCHRONIZE_PROFILE));
            bus.Rpc.Respond<ApplicationFilterModel, ApplicationsPagedListModel>(async (f) => await applicationService.GetApplications(f.Page, f.Size, f.Name, f.ProgramId, f.Faculties, f.Status, f.HaveManager, f.ManagerId, f.SortType),
            x => x.WithQueueName(RabbitMQQueue.GET_APPLICATIONS));

            bus.Rpc.Respond<ManagerQueryModel, List<UserDto>>((query) => managerService.GetManagers(query.Email), x => x.WithQueueName(RabbitMQQueue.GET_MANAGERS));
            bus.Rpc.Respond<Guid, ApplicationDto>(async id => await applicationService.GetApplication(id), x => x.WithQueueName(RabbitMQQueue.GET_APPLICATION));
            bus.Rpc.Respond<EditProgramPriorityDto, Empty>(async data =>
            {
                await applicationService.EditPrograms(data.Programs, data.ApplicationId);
                return new Empty();
            }, x =>  x.WithQueueName(RabbitMQQueue.EDIT_PROGRAMS_PRIORITY));

            bus.Rpc.Respond<ChangeStatusDto, Empty>(async data =>
            {
                await applicationService.ChangeApplicationStatus(data.ApplicationId, data.Status);

                return new Empty();
            }, x => x.WithQueueName(RabbitMQQueue.CHANGE_APPLICATION_STATUS));

            bus.Rpc.Respond<Guid, Empty>(async id =>
            {
                await applicationService.ReturnApplication(id);

                return new Empty();
            }, x => x.WithQueueName(RabbitMQQueue.RETURN_APPLICATION));

            bus.Rpc.Respond<SetManagerApplicationDto, Empty>(async data =>
            {
                await applicationService.TakeApplication(data.ManagerId, data.ApplicationId);

                return new Empty();
            }, x => x.WithQueueName(RabbitMQQueue.SET_MANAGER));
        }

        public async static void Migrate(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<AdmissionDBContext>();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await context.Database.MigrateAsync();
        }
    }
}
