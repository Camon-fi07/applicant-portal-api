using Common.Dto;
using Common.Dto.RabbitMQ;
using Document.BL.Services;
using Document.BL.Services.IServices;
using Document.DAL;
using EasyNetQ;
using Microsoft.AspNetCore.Builder;
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

        public async static void Migrate(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<DocumentDBContext>();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await context.Database.MigrateAsync();
        }
    }
}
