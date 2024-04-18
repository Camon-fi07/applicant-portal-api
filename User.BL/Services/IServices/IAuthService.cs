using Common.Dto;
using Common.Models;
using User.BL.Dto;

namespace User.BL.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokenResponseDto> Login(LoginDto loginDto);
        public Task<TokenResponseDto> RegisterUser(UserRegisterWithRoleModel userRegisterModel);
        public Task Logout(string token);
    }
}
