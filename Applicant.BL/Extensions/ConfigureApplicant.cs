using Applicant.DAL;
using Common.Dto.RabbitMQ;
using Common.Dto;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Applicant.BL.Services.IServices;
using Common.Dto.Applicant;

namespace Applicant.BL.Extensions
{
    public static class ConfigureApplicant
    {
        public static void AddApplicantDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicantDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var applicantService = serviceProvider.GetRequiredService<IApplicantService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.Rpc.Respond<Guid, PassportDto>(async userId => await applicantService.GetPassport(userId));
            bus.Rpc.Respond<Guid, EducationDocumentDto>(async userId => await applicantService.GetEducationDocument(userId));
            bus.Rpc.Respond<Guid, ApplicantProfileDto>(async userId => await applicantService.GetProfile(userId));

            bus.PubSub.Subscribe<RegisterApplicantDto>("applicant_register", data => applicantService.RegisterApplicant(data));
        }
    }
}
