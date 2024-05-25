using Common.Constants;
using Common.Dto;
using Common.Dto.Dictionary;
using Common.Dto.RabbitMQ;
using Dictionary.BL.Mappers;
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

            bus.Rpc.Respond<Guid, AddProgramDto>(async (id) => ProgramMapper.TranslateProgramDtoToAddDto(await gettingService.GetCertainProgram(id)),
                x => x.WithQueueName(RabbitMQQueue.ADD_PROGRAM));

            bus.Rpc.Respond<Guid, AddFacultyDto>(async (id) => FacultyMapper.TranslateFacultyDtoToAddDto(await gettingService.GetCertainFaculty(id)),
                x => x.WithQueueName(RabbitMQQueue.ADD_FACULTY));

            bus.Rpc.Respond<Guid, AddEducationDocumentTypeDto>(async (id) => EducationDocumentTypeMapper.TranslateDocumnetDtoToAddDto(await gettingService.GetCertainDocumentType(id)),
                x => x.WithQueueName(RabbitMQQueue.ADD_EDUCATION_DOCUMENT_TYPE));

            bus.Rpc.Respond<List<Guid>, List<AddProgramDto>>(async (ids) => await gettingService.SychronizePrograms(ids), x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_PROGRAMS));

            bus.Rpc.Respond<List<Guid>, List<AddFacultyDto>>(async (ids) => await gettingService.SynchronizeFaculties(ids), x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_FACULTIES));

            bus.Rpc.Respond<List<int>, List<AddEducationLevelDto>>(async (ids) => await gettingService.SychronizeEducationLevels(ids), x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_EDUCATIONS));

            bus.Rpc.Respond<List<Guid>, List<AddEducationDocumentTypeDto>>(async (ids) => await gettingService.SychronizeDocumentTypes(ids), x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_EDUCATION_DOCUMENT_TYPES));

            bus.Rpc.Respond<Empty, List<EducationProgramDto>>(async (_) => await gettingService.GetAllPrograms(), x => x.WithQueueName(RabbitMQQueue.GET_ALL_PROGRAMS));
            bus.Rpc.Respond<Empty, List<EducationFacultyDto>>(async (_) => await gettingService.GetFaculties(null), x => x.WithQueueName(RabbitMQQueue.GET_FACULTIES));

            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_PROGRAMS, async (_) => await importingService.ImportPrograms(), x => x.WithTopic(RabbitMQQueue.IMPORT_PROGRAMS));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_FACULTIES, async (_) => await importingService.ImportFaculties(), x => x.WithTopic(RabbitMQQueue.IMPORT_FACULTIES));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_EDUCATION_LEVELS, async (_) => await importingService.ImportEducationLevels(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_LEVELS));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS, async (_) => await importingService.ImporDocumentTypes(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS));
        }
    }
}
