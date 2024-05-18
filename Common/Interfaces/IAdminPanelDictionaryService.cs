using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAdminPanelDictionaryService
    {
        public Task ImportEducationDocuments();
        public Task ImportPrograms();
        public Task ImportFaculties();
        public Task ImportEducationLevels();
        public Task<List<ImportingStatusDto>> GetImportingStatuses();
    }
}
