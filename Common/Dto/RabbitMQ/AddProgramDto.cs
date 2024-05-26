using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class AddProgramDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Code { get; set; }
        public required AddEducationLevelDto EducationLevel { get; set; }
        public required AddFacultyDto Faculty { get; set; }
    }
}
