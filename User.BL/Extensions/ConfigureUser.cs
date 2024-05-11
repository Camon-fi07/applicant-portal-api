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

            bus.PubSub.Subscribe<ChangeRoleDto>(RabbitMQQueue.ADD_ROLE, (addRoleDto) =>
            {
                profileService.AddRole(addRoleDto.Role, addRoleDto.UserId);
            }, x => x.WithTopic(RabbitMQQueue.ADD_ROLE));

            bus.PubSub.Subscribe<ChangeRoleDto>(RabbitMQQueue.DELETE_ROLE, (addRoleDto) =>
            {
                profileService.DeleteRole(addRoleDto.Role, addRoleDto.UserId);
            }, x => x.WithTopic(RabbitMQQueue.DELETE_ROLE));

            bus.Rpc.Respond<Guid, AddUserDto>(async request => { 
                var profile = await profileService.GetProfile(request);

                return new AddUserDto()
                {
                    Email = profile.Email,
                    Id = request,
                    Firstname = profile.FirstName,
                    Lastname = profile.LastName,
                    Middlename = profile.MiddleName
                };
            }, x => x.WithQueueName(RabbitMQQueue.ADD_USER));
            bus.Rpc.Respond<Guid, ProfileDto>(async request => await profileService.GetProfile(request), x=> x.WithQueueName(RabbitMQQueue.GET_PROFILE));
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
    }
}
