using User.BL.Dto.Auth;

namespace User.BL.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokenResponseDto> Login(LoginDto loginDto);
        public Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerApplicantDto);
        public Task<TokenResponseDto> RegisterManager(RegisterManagerDto registerManagerDto);
        public Task Logout(string token);
    }
}
