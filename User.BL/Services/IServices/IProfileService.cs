using Common.Enums;
using Common.Dto;
using User.BL.Dto;
using Common.Dto.Applicant;

namespace User.BL.Services.IServices
{
    public interface IProfileService
    {
        public Task<List<RoleType?>> GetUserRoles(Guid userId);
        public Task<ProfileDto> GetProfile(Guid userId);
        public Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto);
        public Task<ApplicantProfileDto> EditApplicantProfile(Guid userId, EditApplicantProfileDto editApplicantProfileDto);
        public Task<ApplicantProfileDto> GetApplicantProfile(Guid userId);
        public Task ResetPassword(Guid userId, ResetPasswordDto resetPasswordDto);
        public Task AddRole(RoleType roleType, Guid userId);
        public Task DeleteRole(RoleType roleType, Guid userId);
    }
}
