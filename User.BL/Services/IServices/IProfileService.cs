using Common.Dto.Profile;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.BL.Services.IServices
{
    public interface IProfileService
    {
        public Task<List<RoleType?>> GetUserRoles(string token);
        public Task<ProfileDto> GetProfile(string token);
    }
}
