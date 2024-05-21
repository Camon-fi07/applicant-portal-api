using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Application
{
    public class ProgramPriorityDto
    {
        public Guid ProgramId { get; set; }
        public required string ProgramName { get; set; }
        public string? ProgramCode { get; set; }
        public Guid FacultyId { get; set; }
        public required string EducationLevelName { get; set; }
        public required string FacultyName { get; set; }
        public int Priority { get; set; }
    }
}
