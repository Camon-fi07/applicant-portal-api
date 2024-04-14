using User.BL.Dto.Profile;
using Common.Enums;

namespace User.BL.Services.IServices
{
    public interface IProfileService
    {
        public Task<List<RoleType?>> GetUserRoles(string token);
        public Task<ProfileDto> GetProfile(string token);
        public Task EditApplicantProfile(string token, EditApplicantProfileDto editApplicantProfileDto);
        public Task EditManagerProfile(string token, EditManagerDto editManagerDto);

    }
}
