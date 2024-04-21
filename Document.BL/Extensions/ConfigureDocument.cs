using Document.BL.Services;
using Document.BL.Services.IServices;
using Document.DAL;
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
    }
}
