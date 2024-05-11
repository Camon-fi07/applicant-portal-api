using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Dto
{
    public class ApplicationShortDto
    {
        public UserDto User { get; set; }
        public Guid ApplicationId { get; set; }
        public ApplicationStatus Status { get; set; }
        public List<ProgramPriorityDto> ProgramPriorities { get; set; }
    }
}
