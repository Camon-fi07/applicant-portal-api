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

            bus.Rpc.Respond<UploadSingleFileDto, Guid>(async request => await documentService.UploadFile(request.File), configuration => configuration.WithQueueName("documentUpload"));

            bus.Rpc.Respond<UploadFilesDto, List<Guid>>(async request => await documentService.UploadFiles(request.File), configuration => configuration.WithQueueName("documentUpload"));

            bus.Rpc.Respond<GetFileDto, FileResponseDto>(async request => await documentService.GetFile(request.Id));

            bus.PubSub.Subscribe<DeleteFilesDto>("documentDelete", (deleteFileDto) =>
            {
                documentService.DeleteFiles(deleteFileDto.FilesIds);
            });

            bus.PubSub.Subscribe<DeleteSingleFileDto>("documentDelete", (deleteFileDto) =>
            {
                documentService.DeleteFile(deleteFileDto.FileId);
            });

            bus.PubSub.Subscribe<ChangeFileDto>("documentChange", (changeFileDto) =>
            {
                documentService.ChangeFile(changeFileDto.File, changeFileDto.FileId);
            });
        }
    }
}
