using User.BL.Services.IServices;
using User.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using User.DAL;
using Microsoft.AspNetCore.Http;
using EasyNetQ;
using Common.Dto.User;
using Microsoft.EntityFrameworkCore;
using Common.Constants;
using Common.Models;

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
            
            if (user == null) throw new CustomExceptionModel(ErrorMessages.USER_NOT_FOUND, 400);
            var refreshTokens = _userDBContext.RefreshTokens.Include(rt => rt.User);

            var identity = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!identity.Succeeded) throw new CustomExceptionModel(ErrorMessages.INVALID_CREDINTAILS, 400);
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
            if (user == null) throw new CustomExceptionModel(ErrorMessages.USER_NOT_FOUND, 400); 

            var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.UserAgent == userAgent);
            if (refreshToken == null) throw new CustomExceptionModel(ErrorMessages.SESSION, 401);

            refreshToken.isRevoked = true;

            await _userDBContext.SaveChangesAsync();
        }

        public async Task<TokenResponseDto> RegisterUser(UserRegisterDto userRegisterDto, string userAgent)
        {
            if (await _userManager.FindByEmailAsync(userRegisterDto.Email) != null)
            {
                throw new CustomExceptionModel(ErrorMessages.EMAIL_DUBLICATE, 400);
            }

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Gender = userRegisterDto.Gender,
                Lastname = userRegisterDto.Lastname,
                Middlename = userRegisterDto.Middlename,
                Firstname = userRegisterDto.Firstname,
                BirthDate = userRegisterDto.BirthDate,
                Email = userRegisterDto.Email,
                PhoneNumber = userRegisterDto.PhoneNumber,
                UserName = userRegisterDto.Email
            };

            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

            if (!result.Succeeded)  throw new CustomExceptionModel($"Произошла ошибка: {result?.Errors.First().Description}", 400);
            
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
