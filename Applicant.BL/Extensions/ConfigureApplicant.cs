using Applicant.DAL;
using Common.Dto.RabbitMQ;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Applicant.BL.Services.IServices;
using Common.Dto.Applicant;
using Common.Constants;
using Common.Enums;
using Common.Dto.User;
using Microsoft.AspNetCore.Builder;

namespace Applicant.BL.Extensions
{
    public static class ConfigureApplicant
    {
        public static void AddApplicantDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicantDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public async static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var applicantService = serviceProvider.GetRequiredService<IApplicantService>();
            var synchronizationService = serviceProvider.GetRequiredService<ISynchronizationService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.Rpc.Respond<Guid, PassportDto>(async userId => await applicantService.GetPassport(userId), x => x.WithQueueName(RabbitMQQueue.GET_PASSPORT));
            bus.Rpc.Respond<Guid, EducationDocumentDto>(async userId => await applicantService.GetEducationDocument(userId), x => x.WithQueueName(RabbitMQQueue.GET_EDUCATION_DOCUMENT));
            bus.Rpc.Respond<Guid, ApplicantDataDto>(async userId => await applicantService.GetApplicantData(userId), x=> x.WithQueueName(RabbitMQQueue.GET_APPLICANT_DATA));
            bus.Rpc.Respond<EditApplicantDataRabbitDto, ApplicantDataDto>(async data => await applicantService.EditApplicantData(data.EditApplicantDataDto, data.UserId), x => x.WithQueueName(RabbitMQQueue.EDIT_APPLICANT_DATA));
            bus.PubSub.Subscribe<GuideType>("Applicant_Guide", async (type) => await synchronizationService.SychronizeGuides(type), x => x.WithQueueName(RabbitMQQueue.REFRESH_GUIDE));


            await bus.PubSub.SubscribeAsync<RegisterApplicantDto>(RabbitMQQueue.APPLICANT_REGISTER, async data => await applicantService.RegisterApplicant(data));
            await bus.PubSub.SubscribeAsync<AddUserDto>("Applicant_Profile", async data => { await synchronizationService.SychronizeProfile(data); }, x => x.WithTopic(RabbitMQQueue.SYNCHRONIZE_PROFILE));
        }

        public async static void Migrate(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<ApplicantDBContext>();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await context.Database.MigrateAsync();
        }
    }
}
