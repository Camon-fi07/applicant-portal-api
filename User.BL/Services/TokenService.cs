using User.BL.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using User.DAL;
using User.BL.Dto;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Common.Dto.User;


namespace User.BL.Services
{
    public class TokenService:ITokenService
    {
        private JwtSecurityTokenHandler _tokenHandler;
        IConfiguration _configuration;
        UserManager<UserEntity> _userManager;
        UserDBContext _userDBContext;
        public TokenService(IConfiguration configuration, UserManager<UserEntity> userManager, UserDBContext userDBContext) {
            _tokenHandler = new JwtSecurityTokenHandler();
            _configuration = configuration;
            _userManager = userManager;
            _userDBContext = userDBContext;
        }

        public async Task<string> GenerateAccessToken(UserEntity user) {
            var tokenProperties = new TokenProperties(); 
            _configuration.GetSection("TokenProperties:accessToken").Bind(tokenProperties);
            return await GenerateToken(user, tokenProperties);
        }

        public async Task<string> GenerateRefreshToken(UserEntity user) {
            var tokenProperties = new TokenProperties();
            _configuration.GetSection("TokenProperties:refreshToken").Bind(tokenProperties);
            return await GenerateToken(user, tokenProperties);
        }

        private async Task<string> GenerateToken(UserEntity user, TokenProperties tokenProperties)
        {
            var key = Encoding.ASCII.GetBytes(tokenProperties.secret);
            var claims = new List<Claim> {
               new Claim(ClaimTypes.Name, user.Id.ToString()),
               new Claim(ClaimTypes.Sid, user.Id.ToString()),
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(tokenProperties.lifetime)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        public async Task<TokenResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto, string userAgent)
        {
            var userId = await GetUserIdByToken(refreshTokenDto.RefreshToken);
            var user = await _userDBContext.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);
            if(user == null) { throw new Exception(); }

            var oldRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.RefreshToken == refreshTokenDto.RefreshToken);

            if (oldRefreshToken == null) throw new UnauthorizedAccessException("Инвалидный токен");

            if (oldRefreshToken.isRevoked)
            {
                foreach (var rt in user.RefreshTokens)
                {
                    rt.isRevoked = true;
                }

                await _userDBContext.SaveChangesAsync();

                throw new Exception("Данный токен уже был использован, все сессии забанены");
            }

            if(oldRefreshToken.UserAgent !=  userAgent) throw new Exception("Вы не можете обновить токен с другого устройства");

            if(oldRefreshToken.ExpiredTime < DateTime.UtcNow) throw new UnauthorizedAccessException("Инвадижный токен");

            oldRefreshToken.isRevoked = true;

            var newRefreshToken = await GenerateRefreshToken(user);

            var newRefreshTokenEntity = new RefreshTokenEntity()
            {
                ExpiredTime = GetTokenExpired(newRefreshToken),
                Id = Guid.NewGuid(),
                isRevoked = false,
                RefreshToken = newRefreshToken,
                User = user,
                UserAgent = userAgent
            };

            _userDBContext.RefreshTokens.Add(newRefreshTokenEntity);

            await _userDBContext.SaveChangesAsync();

            return new TokenResponseDto() { RefreshToken = newRefreshToken, AccessToken = await GenerateAccessToken(user) };
        }

        public DateTime GetTokenExpired(string token)
        {
            if (!_tokenHandler.CanReadToken(token)) throw new UnauthorizedAccessException("Инвалидный токен");

            var jwtToken = _tokenHandler.ReadToken(token) as JwtSecurityToken;

            var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);

            if (expClaim == null) throw new UnauthorizedAccessException("Инвалидный токен");

            var expValue = long.Parse(expClaim.Value);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expValue).UtcDateTime;

            return expirationTime;
        }

        private async Task<Guid> GetUserIdByToken(string token)
        {
            if (!_tokenHandler.CanReadToken(token)) throw new UnauthorizedAccessException("Инвалидный токен");

            var jwtToken = _tokenHandler.ReadToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value;

            if (userId == null) throw new UnauthorizedAccessException("Инвалидный токен");

            return new Guid(userId);

        }
    }
}
