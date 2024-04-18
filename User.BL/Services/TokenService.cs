using User.BL.Models;
using User.BL.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using User.DAL;
using Common.Dto;
using User.BL.Dto;


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
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

        public async Task<TokenResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var user = await GetUserByToken(refreshTokenDto.RefreshToken);

            if (user.RefreshToken != refreshTokenDto.RefreshToken) throw new UnauthorizedAccessException("Инвалидный токен");

            await _userDBContext.BannedTokens.AddAsync(new TokenEntity(){ Token=refreshTokenDto.AccessToken});
            await _userDBContext.SaveChangesAsync();

            return new TokenResponseDto() { RefreshToken = refreshTokenDto.RefreshToken, AccessToken = await GenerateAccessToken(user) };
        }

        public async Task<UserEntity> GetUserByToken(string token)
        {
            if (!_tokenHandler.CanReadToken(token)) throw new UnauthorizedAccessException("Инвалидный токен");

            var jwtToken = _tokenHandler.ReadToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value;

            if (userId == null) throw new UnauthorizedAccessException("Инвалидный токен");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new UnauthorizedAccessException("Инвалидный токен");

            return user;
        }
    }
}
