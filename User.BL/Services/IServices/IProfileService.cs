using Common.Enums;
using Common.Dto;
using User.BL.Dto;

namespace User.BL.Services.IServices
{
    public interface IProfileService
    {
        public Task<List<RoleType?>> GetUserRoles(string token);
        public Task<ProfileDto> GetProfile(string token);
        public Task EditProfile(string token, EditProfileDto editProfileDto);
        public Task ResetPassword(string token, ResetPasswordDto resetPasswordDto);
    }
}
