using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using User.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using User.BL.Models;
using System.IdentityModel.Tokens.Jwt;

namespace User.BL.Extensions
{
    public static class ConfigureAuth
    {
        public static void AddUserDB(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<UserDBContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddAuthenticationCustom(this IServiceCollection services, TokenProperties tokenProperties)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenProperties.secret)),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true
                };
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var token = context.SecurityToken as JwtSecurityToken;
                        context.Request.Headers["Authorization"] = token.RawData;
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
