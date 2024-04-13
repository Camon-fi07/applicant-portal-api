using User.BL.Models;
using User.BL.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace User.BL.Services
{
    public class TokenService:ITokenService
    {
        private JwtSecurityTokenHandler _tokenHandler;
        IConfiguration _configuration;
        public TokenService(IConfiguration configuration) {
            _tokenHandler = new JwtSecurityTokenHandler();
            _configuration = configuration;
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

    }
}
