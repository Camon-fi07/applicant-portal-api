using Common.Dto.User;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAdminPanelProfileService
    {
        public Task<ProfileDto> GetProfile(Guid userId);
        public Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto);
        public Task DeleteManagerRole(Guid id);
        public Task SetManagerRole(Guid id, ManagerRole role);
    }
}
