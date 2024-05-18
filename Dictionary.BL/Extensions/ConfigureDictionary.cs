using Common.Constants;
using Common.Dto;
using Common.Dto.Applicant;
using Common.Dto.RabbitMQ;
using Dictionary.BL.Dto;
using Dictionary.BL.Services.IServices;
using Dictionary.DAL;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dictionary.BL.Extensions
{
    public static class ConfigureDictionary
    {
        public static void AddDictionaryDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DictionaryDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
         
        public async static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var gettingService = serviceProvider.GetRequiredService<IGettingService>();
            var importingService = serviceProvider.GetRequiredService<IImportingService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.Rpc.Respond<Empty, List<ImportingStatusDto>>(async (_) => await gettingService.GetImportingStatuses(), x => x.WithQueueName(RabbitMQQueue.GET_IMPORTING_STATUSES));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_PROGRAMS, async (_) => await importingService.ImportPrograms(), x => x.WithTopic(RabbitMQQueue.IMPORT_PROGRAMS));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_FACULTIES, async (_) => await importingService.ImportFaculties(), x => x.WithTopic(RabbitMQQueue.IMPORT_FACULTIES));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_EDUCATION_LEVELS, async (_) => await importingService.ImportEducationLevels(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_LEVELS));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS, async (_) => await importingService.ImporDocumentTypes(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS));
        }
    }
}
