using Common.Dto.Auth;

namespace User.BL.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokenResponseDto> Login(LoginDto loginData);
        public Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerData);
        public Task<TokenResponseDto> RegisterManager(RegisterManagerDto registerData);
        public Task Logout(string token);
    }
}
