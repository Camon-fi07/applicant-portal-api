using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using User.BL.Models;
using User.DAL;

namespace User.BL.Extensions
{
    public class TokenBlackListHandler:  AuthorizationHandler<TokenBlackListRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenBlackListHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenBlackListRequirement requirement)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userDbContext = scope.ServiceProvider.GetRequiredService<UserDBContext>();

                string token = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                var bannedToken = userDbContext.BannedTokens.FirstOrDefault(t => t.Token == token);

                if (bannedToken != null)
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
