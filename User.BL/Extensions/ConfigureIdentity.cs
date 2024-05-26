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
            services.AddIdentity<UserEntity, Role>()
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

        public static async Task CreateAdmin(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();
            var config = app.Configuration.GetSection("Admin");
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<UserEntity>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "Админ не сконфигурирован");
            }

            var adminUser = await userManager.FindByEmailAsync(config["Email"] ?? throw new ArgumentNullException(
                nameof(config), "Email админа не был найден"));

            var adminGender = config["Gender"] == Gender.Male.ToString() ? Gender.Male : Gender.Female;
            if (adminUser == null)
            {
                var userResult = await userManager.CreateAsync(new UserEntity
                {
                    Lastname = config["Lastname"] ?? throw new ArgumentNullException(
                        nameof(config), "Поле Lastname для админа не было найдено"),
                    Firstname = config["Firstname"] ?? throw new ArgumentNullException(
                        nameof(config), "Поле Firstname для админа не было найдено"),
                    Middlename = config["Middlename"] ?? throw new ArgumentNullException(
                        nameof(config), "Поле Middlename для админа не было найдено"),
                    Email = config["Email"] ?? throw new ArgumentNullException(
                        nameof(config), "Поле Email для админа не было найдено"),
                    UserName = config["Email"] ?? throw new ArgumentNullException(
                        nameof(config), "Поле Email для админа не было найдено"),
                    PhoneNumber = config["PhoneNumber"] ?? throw new ArgumentNullException(
                        nameof(config), "Поле PhoneNumber для админа не было найдено"),
                    Id = Guid.NewGuid(),
                    Gender = adminGender,
                    BirthDate = DateOnly.FromDateTime(DateTime.UtcNow)
                }, config["Password"] ?? throw new ArgumentNullException(
                    nameof(config), "Поле Password для админа не было найдено"));

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException("Не получилось создать администратора");
                }

                adminUser = await userManager.FindByEmailAsync(config["Email"] ?? throw new ArgumentNullException(
                nameof(config), "Email админа не был найден"));
            }

            if (adminUser == null)
            {
                throw new ArgumentNullException(nameof(adminUser), "Не получилось найди администратора");
            }

            if (!await userManager.IsInRoleAsync(adminUser, RoleType.Administrator.ToString()))
            {
                await userManager.AddToRoleAsync(adminUser, RoleType.Administrator.ToString());
            }
        }
    }
}
