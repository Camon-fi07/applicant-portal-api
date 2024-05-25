using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class AddEducationLevelDto
    { 
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
