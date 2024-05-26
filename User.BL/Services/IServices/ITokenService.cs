using Common.Dto.User;
using User.BL.Dto;
using User.DAL.Entities;

namespace User.BL.Services.IServices
{
    public interface ITokenService
    {
        public Task<string> GenerateAccessToken(UserEntity user);
        public Task<string> GenerateRefreshToken(UserEntity user);
        public Task<TokenResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto, string userAgent);
        public DateTime GetTokenExpired(string token);
    }
}
