using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Application
{
    public class CreateProgramPriorityDto
    {
        public Guid ProgramId { get; set; }
        public int Priority { get; set; }
    }
}
