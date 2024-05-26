using Common.Dto.User;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Services.IServices
{
    public interface ISynchronizationService
    {
        public Task SychronizeGuides(GuideType guideType);
        public Task SychronizeEducationDocumentTypes();
        public Task SychronizeEducationLevels();
        public Task SychronizeProfile(AddUserDto addUserDto);
    }
}
