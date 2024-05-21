using Common.Dto;
using Common.Dto.Applicant;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Application
{
    public class ApplicationDto
    {
        public required List<ProgramPriorityDto> ProgramPriorities { get; set; }
        public required Guid ApplicationId { get; set; }
        public UserDto? Manager { get; set; }
        public required UserDto User { get; set; }
        public required ApplicationStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EditTime { get; set; }
    }
}
