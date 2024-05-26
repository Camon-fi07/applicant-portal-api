using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class FacultyCacheEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProgramCacheEntity> Programs { get; set; } = new List<ProgramCacheEntity>();
    }
}
