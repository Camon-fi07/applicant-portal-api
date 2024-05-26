using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Application
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Lastname { get; set; }
        public required string Middlename { get; set; }
        public required string Firstname { get; set; }
    }
}
