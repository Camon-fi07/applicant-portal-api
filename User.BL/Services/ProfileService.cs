using Common.Enums;
using Microsoft.AspNetCore.Identity;
using User.BL.Services.IServices;
using User.DAL.Entities;
using User.DAL;
using Microsoft.AspNetCore.Http;
using User.BL.Dto;
using Common.Dto;

namespace User.BL.Services
{
    public class ProfileService:IProfileService
    {
        ITokenService _tokenService;
        UserManager<UserEntity> _userManager;
        UserDBContext _userDBContext;
        public ProfileService(ITokenService tokenService, UserManager<UserEntity> userManager, UserDBContext userDBContext) {
            _tokenService = tokenService;   
            _userManager = userManager;
            _userDBContext = userDBContext;
        }

        public async Task<List<RoleType?>> GetUserRoles(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if(user == null) { throw new Exception(); }

            var roles = (await _userManager.GetRolesAsync(user)).Select(role => {
                var roleType = GetRoleType(role);
                if (roleType is null) throw new UnauthorizedAccessException();

                return roleType!;
            });

            return roles.ToList();
        }

        public async Task<ProfileDto> GetProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { throw new Exception(); }

            var roles = (await _userManager.GetRolesAsync(user)).Select(role => {
                var roleType = GetRoleType(role);
                if (roleType is null) throw new UnauthorizedAccessException();

                return roleType!;
            });

            return new ProfileDto()
            {
                BirthDate = user.BirthDate,
                Email = user.Email,
                FirstName = user.Firstname,
                Gender = user.Gender,
                LastName = user.Lastname,
                MiddleName = user.Middlename,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            };
        }

        private RoleType? GetRoleType(string role)
        {
            foreach (var roleType in Enum.GetValues(typeof(RoleType)).Cast<RoleType>())
            {
                if (roleType.ToString() == role) return roleType;
            }
            return null;
        }

        public async Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { throw new Exception(); }

            if (editProfileDto.Lastname != null) user.Lastname = editProfileDto.Lastname;
            if (editProfileDto.PhoneNumber != null) user.PhoneNumber = editProfileDto.PhoneNumber;
            if (editProfileDto.Firstname != null) user.Firstname = editProfileDto.Firstname;
            if(editProfileDto.Middlename != null) user.Middlename = editProfileDto.Middlename;
            if(editProfileDto.Gender != null) user.Gender = (Gender)editProfileDto.Gender;
            if(editProfileDto.BirthDate != null) user.BirthDate = (DateOnly)editProfileDto.BirthDate;

            if(editProfileDto.Email != null) {
                user.Email = editProfileDto.Email;
                user.UserName = editProfileDto.Email;

                var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(user, editProfileDto.Email);
                var emailChangeResult = await _userManager.ChangeEmailAsync(user, editProfileDto.Email, emailChangeToken);

                if (!emailChangeResult.Succeeded)
                {
                    throw new BadHttpRequestException("Произошла ошибка");
                }
            }

            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();

            return await GetProfile(userId);
        }

        public async Task ResetPassword(string token, ResetPasswordDto resetPasswordDto)
        {
            var user = await _tokenService.GetUserByToken(token);
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                throw new BadHttpRequestException("Произошла ошибка");
            }

            user.RefreshToken = null;
            await _userDBContext.BannedTokens.AddAsync(new TokenEntity() { Token = token });
            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();
        }

        public async Task AddRole(RoleType role, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            await _userManager.AddToRoleAsync(user, role.ToString());
        }
    }
}
