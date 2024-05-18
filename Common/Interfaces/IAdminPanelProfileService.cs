using Common.Dto;
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

    }
}
