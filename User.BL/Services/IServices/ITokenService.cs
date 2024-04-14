using User.BL.Dto.Auth;
using User.DAL.Entities;

namespace User.BL.Services.IServices
{
    public interface ITokenService
    {
        public Task<string> GenerateAccessToken(UserEntity user);
        public Task<string> GenerateRefreshToken(UserEntity user);
        public Task<TokenResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto);
        public Task<UserEntity> GetUserByToken(string token);
    }
}
