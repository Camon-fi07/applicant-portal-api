using Common.Constants;
using Common.Dto;
using Common.Dto.User;
using Common.Interfaces;
using EasyNetQ;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace AdminPanel.BL.Services
{
    public class AdminPanelAccountService : IAdminPanelAccountService
    {
        private readonly IBus _bus;
        public AdminPanelAccountService(IBus bus) { 
            _bus = bus;
        }

        public async Task<TokenResponseDto> Login(LoginDto loginDto)
        {
            var tokens = await _bus.Rpc.RequestAsync<LoginDto, TokenResponseDto>(loginDto, x => x.WithQueueName(RabbitMQQueue.LOGIN));
            
            return tokens;
        }
    }
}
