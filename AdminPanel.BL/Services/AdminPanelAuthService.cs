using Common.Constants;
using Common.Dto;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using Common.Interfaces;
using EasyNetQ;

namespace AdminPanel.BL.Services
{
    public class AdminPanelAuthService : IAdminPanelAuthService
    {
        private readonly IBus _bus;
        public AdminPanelAuthService(IBus bus) { 
            _bus = bus;
        }

        public async Task<TokenResponseDto> Login(LoginDto loginDto, string userAgent)
        {
            var loginRabbitDto = new LoginRabbitDto()
            {
                LoginDto = loginDto,
                UserAgent = userAgent
            };
            var tokens = await _bus.Rpc.RequestAsync<LoginRabbitDto, TokenResponseDto>(loginRabbitDto, x => x.WithQueueName(RabbitMQQueue.LOGIN));
            
            return tokens;
        }

        public async Task Logout(Guid userId, string userAgent)
        {
            var logoutRabbitDto = new LogoutRabbitDto()
            {
                UserId = userId,
                UserAgent = userAgent
            };
            await _bus.Rpc.RequestAsync<LogoutRabbitDto, Empty>(logoutRabbitDto, x => x.WithQueueName(RabbitMQQueue.LOGOUT));
        }
    }
}
