using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using User.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using User.BL.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;

namespace User.BL.Extensions
{
    public static class ConfigureAuth
    {
        public static void AddUserDB(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<UserDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddAuthenticationCustom(this IServiceCollection services, IConfiguration configuration)
        {
            TokenProperties tokenProperties = new();
            configuration.GetSection("TokenProperties:accessToken").Bind(tokenProperties);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenProperties.secret)),
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var token = (JsonWebToken)context.SecurityToken;
                        context.Request.Headers["Authorization"] = token.EncodedToken;
                        return Task.CompletedTask;
                    },
                };
            });

            services.AddSingleton<IAuthorizationHandler, TokenBlackListHandler>();

            services.AddAuthorizationBuilder()
                .AddPolicy("TokenNotInBlackList", policy => policy.Requirements.Add(new TokenBlackListRequirement()));
        }

    }
}
