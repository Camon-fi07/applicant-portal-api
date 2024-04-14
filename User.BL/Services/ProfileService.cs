using User.BL.Dto.Profile;
using Common.Enums;
using Microsoft.AspNetCore.Identity;
using User.BL.Services.IServices;
using User.DAL.Entities;
using User.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace User.BL.Services
{
    public class ProfileService:IProfileService
    {
        ITokenService _tokenService;
        UserManager<UserEntity> _userManager;
        RoleManager<Role> _roleManager;
        UserDBContext _userDBContext;
        public ProfileService(ITokenService tokenService, UserManager<UserEntity> userManager, RoleManager<Role> roleManager, UserDBContext userDBContext) {
            _tokenService = tokenService;   
            _userManager = userManager;
            _roleManager = roleManager; 
            _userDBContext = userDBContext;
        }

        public async Task<List<RoleType?>> GetUserRoles(string token)
        {
            var user = await _tokenService.GetUserByToken(token);

            var roles = (await _userManager.GetRolesAsync(user)).Select(role => {
                var roleType = GetRoleType(role);
                if (roleType is null) throw new UnauthorizedAccessException();

                return roleType!;
            });

            return roles.ToList();
        }

        public async Task<ProfileDto> GetProfile(string token)
        {
            var user = await _tokenService.GetUserByToken(token);

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

        public async Task EditApplicantProfile(string token, EditApplicantProfileDto editApplicantProfileDto)
        {
            var user = await _tokenService.GetUserByToken(token);
            var applicant = await _userDBContext.Applicants.FirstOrDefaultAsync(applicant => applicant.User.Id == user.Id);

            if(applicant == null)
            {
                throw new BadHttpRequestException("Пользователь не абитуриент");
            }

            user.Lastname = editApplicantProfileDto.Lastname;
            user.PhoneNumber = editApplicantProfileDto.PhoneNumber;
            user.Firstname = editApplicantProfileDto.Firstname;
            user.Middlename = editApplicantProfileDto.Middlename;
            user.Gender = editApplicantProfileDto.Gender;
            user.BirthDate = editApplicantProfileDto.BirthDate;
            applicant.Citizenship = editApplicantProfileDto.Citizenship;

            user.Email = editApplicantProfileDto.Email;
            user.UserName = editApplicantProfileDto.Email;
            var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(user, editApplicantProfileDto.Email);
            var emailChangeResult = await _userManager.ChangeEmailAsync(user, editApplicantProfileDto.Email, emailChangeToken);

            if (!emailChangeResult.Succeeded)
            {
                throw new BadHttpRequestException("Произошла ошибка");
            }

            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();
        }

        public async Task EditManagerProfile(string token, EditManagerDto editManagerDto)
        {
            var user = await _tokenService.GetUserByToken(token);
            var manager = await _userDBContext.Managers.FirstOrDefaultAsync(manager => manager.User.Id == user.Id);

            user.Email = editManagerDto.Email;
            user.UserName = editManagerDto.Email;
            var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(user, editManagerDto.Email);
            var emailChangeResult = await _userManager.ChangeEmailAsync(user, editManagerDto.Email, emailChangeToken);

            if (!emailChangeResult.Succeeded)
            {
                throw new BadHttpRequestException("Произошла ошибка");
            }

            await _userManager.UpdateAsync(user);
        }
    }
}
