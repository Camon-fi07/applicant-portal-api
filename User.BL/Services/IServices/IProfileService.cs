using Common.Enums;
using Common.Dto;
using User.BL.Dto;

namespace User.BL.Services.IServices
{
    public interface IProfileService
    {
        public Task<List<RoleType?>> GetUserRoles(Guid userId);
        public Task<ProfileDto> GetProfile(Guid userId);
        public Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto);
        public Task ResetPassword(string token, ResetPasswordDto resetPasswordDto);
        public Task AddRole(RoleType roleType, Guid userId);
    }
}
