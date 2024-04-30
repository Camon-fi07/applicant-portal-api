using Common.Dto;
using Common.Dto.RabbitMQ;
using Document.BL.Services;
using Document.BL.Services.IServices;
using Document.DAL;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Document.BL.Extensions
{
    public static class ConfigureDocument
    {
        public static void AddDocumentDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DocumentDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddDocumentServices(this IServiceCollection services)
        {
            services.AddScoped<IDocumentService, DocumentService>();
        }

        public static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var documentService = serviceProvider.GetRequiredService<IDocumentService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.Rpc.Respond<UploadSingleFileDto, FileResponseDto>(async request => await documentService.UploadFile(request.File), configuration => configuration.WithQueueName("documentUpload"));

            bus.Rpc.Respond<UploadFilesDto, List<FileResponseDto>>(async request => await documentService.UploadFiles(request.Files), configuration => configuration.WithQueueName("documentUpload"));

            bus.Rpc.Respond<GetSingleFileDto, FileResponseDto>(async request => await documentService.GetFile(request.Id), configuration => configuration.WithQueueName("documentGet"));
            bus.Rpc.Respond<GetFilesDto, List<FileResponseDto>>(async request => await documentService.GetFiles(request.Ids), configuration => configuration.WithQueueName("documentGet"));

            bus.Rpc.Respond<ChangeFileDto, FileResponseDto>(async changeFileDto => await documentService.ChangeFile(changeFileDto), configuration => configuration.WithQueueName("documentChange"));
            bus.Rpc.Respond<List<ChangeFileDto>, List<FileResponseDto>>(async changeFileDto => await documentService.ChangeFiles(changeFileDto), configuration => configuration.WithQueueName("documentChange"));


            bus.PubSub.Subscribe<DeleteFilesDto>("documentDelete", async (deleteFileDto) =>
            {
                await documentService.DeleteFiles(deleteFileDto.FilesIds);
            });

            bus.PubSub.Subscribe<DeleteSingleFileDto>("documentDelete", async (deleteFileDto) =>
            {
                await documentService.DeleteFile(deleteFileDto.FileId);
            });
        }
    }
}
