using Common.Dto.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class EditProgramPriorityDto
    {
        public List<CreateProgramPriorityDto> Programs { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
