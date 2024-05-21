using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class AddEducationDocumentTypeRabbitDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required AddEducationLevelRabbitDto EducationLevel { get; set; }
        public List<AddEducationLevelRabbitDto>? NextEducationLevels { get; set; }
    }
}
