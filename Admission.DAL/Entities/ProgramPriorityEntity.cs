using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class ProgramPriorityEntity
    {
        public Guid Id { get; set; }
        public Guid ProgramId { get; set; }
        public Guid FacultyId { get; set; }
        public int Priority { get; set; }
    }
}
