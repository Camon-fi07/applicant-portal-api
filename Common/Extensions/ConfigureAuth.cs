using Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common.Extensions
{
    public static class ConfigureAuth
    {
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
        }
        public static void AddAuthenticationWithCookie(this IServiceCollection services, IConfiguration configuration)
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
                    OnMessageReceived = context =>
                    {
                        var token = context.HttpContext.Request.Cookies["accessToken"];
                        context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
