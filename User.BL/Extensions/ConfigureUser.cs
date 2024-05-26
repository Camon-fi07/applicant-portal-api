using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using User.DAL;
using Common.Dto.RabbitMQ;
using EasyNetQ;
using User.BL.Services.IServices;
using User.BL.Services;
using Common.Dto;
using Common.Constants;
using Common.Dto.User;
using Microsoft.AspNetCore.Builder;
using Common.Models;

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
            var authService = serviceProvider.GetRequiredService<IAuthService>();
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Subscribe<ChangeRoleDto>(RabbitMQQueue.ADD_ROLE, (addRoleDto) =>
            {
                profileService.AddRole(addRoleDto.Role, addRoleDto.UserId);
            }, x => x.WithTopic(RabbitMQQueue.ADD_ROLE));

            bus.PubSub.Subscribe<ChangeRoleDto>(RabbitMQQueue.DELETE_ROLE, (addRoleDto) =>
            {
                profileService.DeleteRole(addRoleDto.Role, addRoleDto.UserId);
            }, x => x.WithTopic(RabbitMQQueue.DELETE_ROLE));

            bus.Rpc.Respond<Guid, AddUserDto>(async request => await profileService.GetShortProfile(request), x => x.WithQueueName(RabbitMQQueue.ADD_USER));

            bus.Rpc.Respond<Guid, ProfileDto>(async request => await profileService.GetProfile(request), x=> x.WithQueueName(RabbitMQQueue.GET_PROFILE));

            bus.Rpc.Respond<LoginRabbitDto, TokenResponseDto>(async request => await authService.Login(request.LoginDto, request.UserAgent), x => x.WithQueueName(RabbitMQQueue.LOGIN));

            bus.Rpc.Respond<EditRabbitProfileDto, ProfileDto>(async request => await profileService.EditProfile(request.UserId, request.EditProfileDto), x => x.WithQueueName(RabbitMQQueue.EDIT_PROFILE));

            bus.Rpc.Respond<string, List<ProfileWithRolesDto>>(async request => await profileService.GetAllUsers(request ?? ""), x => x.WithQueueName(RabbitMQQueue.GET_ALL_USERS));
            
            bus.Rpc.Respond<LogoutRabbitDto, Empty>(async (request) =>
            {
                await authService.Logout(request.UserId, request.UserAgent);
                return new Empty();
            }, x => x.WithQueueName(RabbitMQQueue.LOGOUT));
        }

        public static void AddUserServices(this IServiceCollection services)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            services.AddSingleton(bus);
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddHostedService<BackgroundTokenService>();
        }

        public static async Task Migrate(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<UserDBContext>();
            if (context == null)
            {
                throw new CustomExceptionModel(nameof(context), 500);
            }

            await context.Database.MigrateAsync();
        }
    }
}
