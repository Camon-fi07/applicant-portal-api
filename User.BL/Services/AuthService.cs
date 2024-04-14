using User.BL.Services.IServices;
using User.DAL.Entities;
using User.BL.Dto.Auth;
using Common.Enums;
using Microsoft.AspNetCore.Identity;
using User.DAL;
using Microsoft.AspNetCore.Http;
using User.BL.Models;
using User.BL.Mappers;

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

        public async Task<TokenResponseDto> Login(LoginDto loginDto) { 
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) throw new BadHttpRequestException("Пользователя с таким email не существует");
            var identity = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!identity.Succeeded) throw new BadHttpRequestException("Неверный email или пароль");
            var refreshToken = await _tokenService.GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = await _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {
            var user = await RegisterUser(RegisterMapper.TranslateApplicantRegisterToUserRegister(registerApplicantDto));

            var resultRole = await _userManager.AddToRoleAsync(user, RoleType.Applicant.ToString());

            if (!resultRole.Succeeded) throw new BadHttpRequestException(resultRole.Errors.ToList().ToString());

            var applicant = new Applicant
            {
                Id = Guid.NewGuid(),
                Citizenship = registerApplicantDto.Citizenship,
                User = user,
            };

            await _userDBContext.Applicants.AddAsync(applicant);
            await _userDBContext.SaveChangesAsync();

            var refreshToken = await _tokenService.GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = await _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }

        public async Task Logout(string token) {
            var user = await _tokenService.GetUserByToken(token);

            await _userDBContext.BannedTokens.AddAsync(new TokenEntity() { Token=token});
            user.RefreshToken = null;
            await _userDBContext.SaveChangesAsync();
            await _userManager.UpdateAsync(user);
        }

        public async Task<TokenResponseDto> RegisterManager(RegisterManagerDto registerManagerDto)
        {
            var user = await RegisterUser(RegisterMapper.TranslateManagerRegisterToUserRegister(registerManagerDto));

            var resultRole = await _userManager.AddToRoleAsync(user, RoleType.Manager.ToString());

            if (!resultRole.Succeeded) throw new BadHttpRequestException(resultRole.Errors.ToList().ToString());

            var manager = new Manager
            {
                Id = Guid.NewGuid(),
                User = user,
            };

            await _userDBContext.Managers.AddAsync(manager);
            await _userDBContext.SaveChangesAsync();

            var refreshToken = await _tokenService.GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = await _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }

        private async Task<UserEntity> RegisterUser(UserRegisterModel userRegisterModel)
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

            if (!result.Succeeded) throw new BadHttpRequestException(result.Errors.ToList().ToString());
            await _userManager.UpdateAsync(user);

            return user;
        }
    }
}
