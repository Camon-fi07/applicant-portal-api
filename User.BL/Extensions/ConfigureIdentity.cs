using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using User.DAL.Entities;
using Common.Enums;
using User.DAL;
using System.Data;

namespace User.BL.Extensions
{
    public static class ConfigureIdentity
    {

        public static void ConfigureIdentityDependencies(this IServiceCollection services) { 
            services.AddIdentity<UserEntity, Role>(options => { options.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<UserDBContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<UserEntity>>()
                .AddUserManager<UserManager<UserEntity>>()
                .AddRoleManager<RoleManager<Role>>();
        }

        public static async Task ConfigureRoles(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<UserEntity>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();

            foreach (var role in Enum.GetValues(typeof(RoleType)).Cast<RoleType>())
            {
                var roleFindingResult = await roleManager!.FindByNameAsync(role.ToString());
                if (roleFindingResult == null) {
                    await roleManager.CreateAsync(new Role { Name = role.ToString(), RoleType=role });
                }
            }
        }
    }
}
