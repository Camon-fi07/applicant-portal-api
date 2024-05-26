using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using User.DAL.Entities;
using Common.Enums;
using User.DAL;
using System.Data;
using Common.Models;

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
                throw new CustomExceptionModel("Админ не сконфигурирован", 500);
            }

            var adminUser = await userManager.FindByEmailAsync(config["Email"] ?? throw new CustomExceptionModel(
                "Email админа не был найден", 500));

            var adminGender = config["Gender"] == Gender.Male.ToString() ? Gender.Male : Gender.Female;
            if (adminUser == null)
            {
                var userResult = await userManager.CreateAsync(new UserEntity
                {
                    Lastname = config["Lastname"] ?? throw new CustomExceptionModel(
                        "Поле Lastname для админа не было найдено", 500),
                    Firstname = config["Firstname"] ?? throw new CustomExceptionModel(
                        "Поле Firstname для админа не было найдено", 500),
                    Middlename = config["Middlename"] ?? throw new CustomExceptionModel(
                        "Поле Middlename для админа не было найдено", 500),
                    Email = config["Email"] ?? throw new CustomExceptionModel(
                        "Поле Email для админа не было найдено", 500),
                    UserName = config["Email"] ?? throw new CustomExceptionModel(
                         "Поле Email для админа не было найдено", 500),
                    PhoneNumber = config["PhoneNumber"] ?? throw new CustomExceptionModel(
                        "Поле PhoneNumber для админа не было найдено", 500),
                    Id = Guid.NewGuid(),
                    Gender = adminGender,
                    BirthDate = DateOnly.FromDateTime(DateTime.UtcNow)
                }, config["Password"] ?? throw new CustomExceptionModel(
                    "Поле Password для админа не было найдено", 500));

                if (!userResult.Succeeded)
                {
                    throw new CustomExceptionModel("Не получилось создать администратора", 500);
                }

                adminUser = await userManager.FindByEmailAsync(config["Email"] ?? throw new CustomExceptionModel(
                "Email админа не был найден", 500));
            }

            if (adminUser == null)
            {
                throw new CustomExceptionModel("Не получилось найди администратора", 500);
            }

            if (!await userManager.IsInRoleAsync(adminUser, RoleType.Administrator.ToString()))
            {
                await userManager.AddToRoleAsync(adminUser, RoleType.Administrator.ToString());
            }
        }
    }
}
