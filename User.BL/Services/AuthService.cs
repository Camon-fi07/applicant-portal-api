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
        private readonly UserDBContext _dbContext;

        public AuthService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ITokenService tokenService, UserDBContext dBContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _dbContext = dBContext;
        }

        public async Task<TokenResponseDto> Login(LoginDto loginData) { 
            var user = await _userManager.FindByEmailAsync(loginData.Email);
            if (user == null) throw new BadHttpRequestException("Пользователя с таким email не существует");
            var identity = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, false);

            if (!identity.Succeeded) throw new BadHttpRequestException("Неверный email или пароль");

            return new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user.Id),
                RefreshToken = _tokenService.GenerateRefreshToken(user.Id)
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

            await _dbContext.Applicants.AddAsync(applicant);
            await _dbContext.SaveChangesAsync();

            return new TokenResponseDto() {
                AccessToken = _tokenService.GenerateAccessToken(user.Id),
                RefreshToken = _tokenService.GenerateRefreshToken(user.Id)
            };

        }

        public Task<TokenResponseDto> RegisterManager(RegisterManagerDto registerData)
        {
            throw new NotImplementedException();
        }
    }
}
