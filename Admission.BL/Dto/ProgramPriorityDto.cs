using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Dto
{
    public class ProgramPriorityDto
    {
        public Guid ProgramId { get; set; }
        public Guid FacultyId { get; set; }
        public int Priority { get; set; }
    }
}
