using User.BL.Services.IServices;
using User.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using User.DAL;
using Microsoft.AspNetCore.Http;
using Common.Models;
using EasyNetQ;
using Common.Dto.User;
using Microsoft.EntityFrameworkCore;

namespace User.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly UserDBContext _userDBContext;
        private readonly IBus _bus;

        public AuthService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ITokenService tokenService, UserDBContext userDBContext, IBus bus)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _userDBContext = userDBContext;
            _bus = bus;
        }

        public async Task<TokenResponseDto> Login(LoginDto loginDto, string userAgent) {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null) throw new BadHttpRequestException("Пользователя с таким email не существует");
            var refreshTokens = _userDBContext.RefreshTokens.Include(rt => rt.User);

            var identity = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!identity.Succeeded) throw new BadHttpRequestException("Неверный email или пароль");
            var refreshToken = await _tokenService.GenerateRefreshToken(user);
            
            var oldRefresh = await refreshTokens.FirstOrDefaultAsync(rt => rt.UserAgent == userAgent && rt.User.Id == user.Id);

            if(oldRefresh != null && !oldRefresh.isRevoked)
            {
                oldRefresh.RefreshToken = refreshToken;
            }
            else
            {
                _userDBContext.RefreshTokens.Add(new RefreshTokenEntity()
                {
                    Id = Guid.NewGuid(),
                    isRevoked = false,
                    RefreshToken = refreshToken,
                    User = user,
                    UserAgent = userAgent,
                    ExpiredTime = _tokenService.GetTokenExpired(refreshToken),
                });
            }

            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();
            
            return new TokenResponseDto
            {
                AccessToken = await _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }

        public async Task Logout(Guid userId, string userAgent) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { throw new Exception(); }

            var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.UserAgent == userAgent);
            if (refreshToken == null) throw new UnauthorizedAccessException("Пользователь не имеет сессии на данном устройстве");

            refreshToken.isRevoked = true;

            await _userDBContext.SaveChangesAsync();
        }

        public async Task<TokenResponseDto> RegisterUser(UserRegisterModel userRegisterModel, string userAgent)
        {
            if (await _userManager.FindByEmailAsync(userRegisterModel.Email) != null)
            {
                throw new BadHttpRequestException("Пользователь с таким email уже существует");
            }

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Gender = userRegisterModel.Gender,
                Lastname = userRegisterModel.Lastname,
                Middlename = userRegisterModel.Middlename,
                Firstname = userRegisterModel.Firstname,
                BirthDate = userRegisterModel.BirthDate,
                Email = userRegisterModel.Email,
                PhoneNumber = userRegisterModel.PhoneNumber,
                UserName = userRegisterModel.Email
            };

            var result = await _userManager.CreateAsync(user, userRegisterModel.Password);

            if (!result.Succeeded) throw new BadHttpRequestException("Произошла ошибка");
            await _userManager.UpdateAsync(user);

            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            _userDBContext.RefreshTokens.Add(new RefreshTokenEntity()
            {
                Id = Guid.NewGuid(),
                isRevoked = false,
                RefreshToken = refreshToken,
                User = user,
                UserAgent = userAgent,
                ExpiredTime = _tokenService.GetTokenExpired(refreshToken),
            });

            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = await _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }
    }
}
