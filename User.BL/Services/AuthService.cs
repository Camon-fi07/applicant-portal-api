using User.BL.Services.IServices;
using User.DAL.Entities;
using Common.Dto.Auth;
using Common.Enums;
using Microsoft.AspNetCore.Identity;
using User.DAL;
using Microsoft.AspNetCore.Http;

namespace User.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly UserDBContext _userDBContext;

        public AuthService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ITokenService tokenService, UserDBContext userDBContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _userDBContext = userDBContext;
        }

        public async Task<TokenResponseDto> Login(LoginDto loginData) { 
            var user = await _userManager.FindByEmailAsync(loginData.Email);
            if (user == null) throw new BadHttpRequestException("Пользователя с таким email не существует");
            var identity = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, false);

            if (!identity.Succeeded) throw new BadHttpRequestException("Неверный email или пароль");
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user.Id),
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerData)
        {
            if(await _userManager.FindByEmailAsync(registerData.Email) != null)
            { 
                throw new BadHttpRequestException("Пользователь с таким email уже существует");
            }

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Gender = registerData.Gender,
                Lastname = registerData.Lastname,
                Middlename = registerData.Middlename,
                Firstname = registerData.Firstname,
                BirthDate = registerData.BirthDate,
                Email = registerData.Email,
                PhoneNumber = registerData.PhoneNumber,
                UserName = registerData.Email
            };

            var result = await _userManager.CreateAsync(user, registerData.Password);

            if (!result.Succeeded) throw new BadHttpRequestException(result.Errors.ToList().ToString());

            var resultRole = await _userManager.AddToRoleAsync(user, RoleType.Applicant.ToString());

            if (!resultRole.Succeeded) throw new BadHttpRequestException(resultRole.Errors.ToList().ToString());

            var applicant = new Applicant
            {
                Id = Guid.NewGuid(),
                Citizenship = registerData.Citizenship,
                User = user,
            };

            await _userDBContext.Applicants.AddAsync(applicant);
            await _userDBContext.SaveChangesAsync();

            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user.Id),
                RefreshToken = refreshToken
            };

        }

        public async Task Logout(string token) {
            var userId = _tokenService.GetUserIdByToken(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new UnauthorizedAccessException("Инвалидный токен");

            await _userDBContext.BannedTokens.AddAsync(new TokenEntity() { Token=token});
            user.RefreshToken = null;
            await _userDBContext.SaveChangesAsync();
            await _userManager.UpdateAsync(user);
        }

        public Task<TokenResponseDto> RegisterManager(RegisterManagerDto registerData)
        {
            throw new NotImplementedException();
        }
    }
}
