using Common.Dto;
using Common.Dto.User;
using Common.Models;
using User.BL.Dto;

namespace User.BL.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokenResponseDto> Login(LoginDto loginDto, string userAgent);
        public Task<TokenResponseDto> RegisterUser(UserRegisterModel userRegisterModel, string userAgent);
        public Task Logout(Guid userId, string userAgent);
    }
}
