using Applicant.BL.Services;
using Applicant.BL.Services.IServices;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureApplicantServices(this IServiceCollection services)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            services.AddSingleton(bus);
            services.AddScoped<IExternalService, ExternalService>();
            services.AddScoped<IApplicantService, ApplicantService>();
            services.AddScoped<ISynchronizationService, SynchronizationService>();
        }
    }
}
