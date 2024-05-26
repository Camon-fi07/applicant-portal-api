using Common.Dto.User;
using User.BL.Dto;

namespace User.BL.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokenResponseDto> Login(LoginDto loginDto, string userAgent);
        public Task<TokenResponseDto> RegisterUser(UserRegisterDto userRegisterDto, string userAgent);
        public Task Logout(Guid userId, string userAgent);
    }
}
