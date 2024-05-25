using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Services.IServices
{
    public interface ISynchronizationService
    {
        public Task SychronizeGuides(GuideType guideType);
        public Task SychronizePrograms();
        public Task SychronizeEducationDocumentTypes();
        public Task SychronizeEducationLevels();
        public Task SychronizeFaculties();
    }
}
