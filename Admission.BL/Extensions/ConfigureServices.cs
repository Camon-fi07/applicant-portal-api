using Admission.BL.Services;
using Admission.BL.Services.IServices;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureAdmissionServices(this IServiceCollection services)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            services.AddSingleton(bus); 
            services.AddScoped<IApplicationService, ApplicationService>();
        }
    }
}
