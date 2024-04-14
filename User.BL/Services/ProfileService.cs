using Common.Dto.Profile;
using Common.Enums;
using Microsoft.AspNetCore.Identity;
using User.BL.Services.IServices;
using User.DAL.Entities;

namespace User.BL.Services
{
    public class ProfileService:IProfileService
    {
        ITokenService _tokenService;
        UserManager<UserEntity> _userManager;
        RoleManager<Role> _roleManager;
        public ProfileService(ITokenService tokenService, UserManager<UserEntity> userManager, RoleManager<Role> roleManager) {
            _tokenService = tokenService;   
            _userManager = userManager;
            _roleManager = roleManager; 
        }

        public async Task<List<RoleType?>> GetUserRoles(string token)
        {
            var userId = _tokenService.GetUserIdByToken(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new UnauthorizedAccessException("Инвалидный токен");

            var roles = (await _userManager.GetRolesAsync(user)).Select(role => {
                var roleType = GetRoleType(role);
                if (roleType is null) throw new UnauthorizedAccessException();

                return roleType!;
            });

            return roles.ToList();
        }

        public async Task<ProfileDto> GetProfile(string token)
        {
            var userId = _tokenService.GetUserIdByToken(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null) throw new UnauthorizedAccessException("Инвалидный токен");

            var roles =( await _userManager.GetRolesAsync(user)).Select(role => {
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
    }
}
