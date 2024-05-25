using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class EducationLevelCacheEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<ProgramCacheEntity> Programs { get; set; } = new List<ProgramCacheEntity>();
        public List<EducationDocumentTypeCacheEntity>? CurrentEducationLevelDocumentTypes { get; set; }
        public List<EducationDocumentTypeCacheEntity>? PreviousEducationLevelDocumentTypes { get; set; }
    }
}
