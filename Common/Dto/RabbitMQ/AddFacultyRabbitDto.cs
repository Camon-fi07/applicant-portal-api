using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class AddFacultyRabbitDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
