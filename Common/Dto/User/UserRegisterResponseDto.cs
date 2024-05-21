using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.User
{
    public class UserRegisterResponseDto
    {
        public TokenResponseDto TokenResponseDto { get; set; }
        public Guid UserId { get; set; }
    }
}
