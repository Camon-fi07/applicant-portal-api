using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.User
{
    public class AddUserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Lastname { get; set; }
        public required string Firstname { get; set; }
        public required string Middlename { get; set; }
    }
}
