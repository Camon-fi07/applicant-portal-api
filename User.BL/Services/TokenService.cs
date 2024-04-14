using User.BL.Models;
using User.BL.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Dto.Auth;
using User.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;


namespace User.BL.Services
{
    public class TokenService:ITokenService
    {
        private JwtSecurityTokenHandler _tokenHandler;
        IConfiguration _configuration;
        UserManager<UserEntity> _userManager;
        public TokenService(IConfiguration configuration, UserManager<UserEntity> userManager) {
            _tokenHandler = new JwtSecurityTokenHandler();
            _configuration = configuration;
            _userManager = userManager;
        }

        public string GenerateAccessToken(Guid userId) {
            var tokenProperties = new TokenProperties(); 
            _configuration.GetSection("TokenProperties:accessToken").Bind(tokenProperties);
            return GenerateToken(userId, tokenProperties);
        }

        public string GenerateRefreshToken(Guid userId) {
            var tokenProperties = new TokenProperties();
            _configuration.GetSection("TokenProperties:refreshToken").Bind(tokenProperties);
            return GenerateToken(userId, tokenProperties);
        }

        private string GenerateToken(Guid id, TokenProperties tokenProperties)
        {
            var key = Encoding.ASCII.GetBytes(tokenProperties.secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, id.ToString()),
                }),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(tokenProperties.lifetime)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        public async Task<TokenResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var userId = GetUserIdByToken(refreshTokenDto.RefreshToken);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken) throw new UnauthorizedAccessException("Инвалидный токен");

            return new TokenResponseDto() { RefreshToken = refreshTokenDto.RefreshToken, AccessToken = GenerateAccessToken(userId) };
        }

        public Guid GetUserIdByToken(string token)
        {
            if (!_tokenHandler.CanReadToken(token)) throw new UnauthorizedAccessException("Инвалидный токен");

            var jwtToken = _tokenHandler.ReadToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value;

            if (userId == null) throw new UnauthorizedAccessException("Инвалидный токен");

            return new Guid(userId);
        }
    }
}
