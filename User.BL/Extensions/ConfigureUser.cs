using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using User.DAL;
using Common.Dto.RabbitMQ;
using EasyNetQ;
using User.BL.Services.IServices;
using User.BL.Services;
using Common.Dto;

namespace User.BL.Extensions
{
    public static class ConfigureUser
    {
        public static void AddUserDB(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<UserDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddEasyNetQ(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var profileService = serviceProvider.GetRequiredService<IProfileService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Subscribe<AddRoleDto>("userAddRole", (addRoleDto) =>
            {
                profileService.AddRole(addRoleDto.Role, addRoleDto.UserId);
            });

            bus.Rpc.Respond<Guid, ProfileDto>(async request => await profileService.GetProfile(request));
            bus.Rpc.Respond<EditUserProfileDto, ProfileDto>(async request => await profileService.EditProfile(request.UserId, request.EditProfileDto));
        }

        public static void AddUserServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddHostedService<BackgroundTokenService>();
        }
    }
}
