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

            bus.Rpc.Respond<Guid, AddProgramRabbitDto>(async (id) => {
                var programDto = await gettingService.GetProgram(id);

                var addEducationLevel = new AddEducationLevelRabbitDto()
                {
                    Id = programDto.EducationLevel.Id,
                    Name = programDto.EducationLevel.Name,
                };

                var addFaculty = new AddFacultyRabbitDto()
                {
                    Id = programDto.Faculty.Id,
                    Name = programDto.Faculty.Name,
                };

                return new AddProgramRabbitDto()
                {
                    Code = programDto.Code,
                    EducationLevel = addEducationLevel,
                    Name = programDto.Name,
                    Id = programDto.Id,
                    Faculty = addFaculty
                };

            }, x => x.WithQueueName(RabbitMQQueue.ADD_PROGRAM));

            bus.Rpc.Respond<Guid, AddFacultyRabbitDto>(async (id) => {
                var faculty = await gettingService.GetFaculty(id);

                return new AddFacultyRabbitDto()
                {
                    Id = faculty.Id,
                    Name = faculty.Name,
                };

            }, x => x.WithQueueName(RabbitMQQueue.ADD_FACULTY));

            bus.Rpc.Respond<Guid, AddEducationDocumentTypeRabbitDto>(async (id) => {
                var educationDocument = await gettingService.GetDocumentType(id);

                var educationLevel = new AddEducationLevelRabbitDto()
                {
                    Id = educationDocument.EducationLevel.Id,
                    Name = educationDocument.EducationLevel.Name,
                };

                var nextEducationsLevels = educationDocument.NextEducationLevels?.Select(e => new AddEducationLevelRabbitDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                }).ToList();

                return new AddEducationDocumentTypeRabbitDto()
                {
                    EducationLevel = educationLevel,
                    Id = educationDocument.Id,
                    Name = educationDocument.Name,
                    NextEducationLevels = nextEducationsLevels
                };

            }, x => x.WithQueueName(RabbitMQQueue.ADD_EDUCATION_DOCUMENT_TYPE));

            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_PROGRAMS, async (_) => await importingService.ImportPrograms(), x => x.WithTopic(RabbitMQQueue.IMPORT_PROGRAMS));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_FACULTIES, async (_) => await importingService.ImportFaculties(), x => x.WithTopic(RabbitMQQueue.IMPORT_FACULTIES));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_EDUCATION_LEVELS, async (_) => await importingService.ImportEducationLevels(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_LEVELS));
            bus.PubSub.Subscribe<Empty>(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS, async (_) => await importingService.ImporDocumentTypes(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS));
        }
    }
}
