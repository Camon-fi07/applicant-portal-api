using Common.RabbitMQ;
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
            services.AddScoped<IRabbitMqService, RabbitMqService>();
        }
    }
}
