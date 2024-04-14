using Common.Dto.Auth;
using User.DAL.Entities;

namespace User.BL.Services.IServices
{
    public interface ITokenService
    {
        public string GenerateAccessToken(Guid userId);
        public string GenerateRefreshToken(Guid userId);
        public Task<TokenResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto);
        public Guid GetUserIdByToken(string token);

    }
}
