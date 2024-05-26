using Common.Enums;
using User.BL.Dto;
using Common.Dto.Applicant;
using Common.Dto.User;

namespace User.BL.Services.IServices
{
    public interface IProfileService
    {
        public Task<List<RoleType>> GetUserRoles(Guid userId);
        public Task<ProfileDto> GetProfile(Guid userId);
        public Task<AddUserDto> GetShortProfile(Guid userId);
        public Task<ProfileWithRolesDto> GetProfileWithRoles(Guid userId);
        public Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto);
        public Task ResetPassword(Guid userId, ResetPasswordDto resetPasswordDto);
        public Task AddRole(RoleType roleType, Guid userId);
        public Task DeleteRole(RoleType roleType, Guid userId);
    }
}
