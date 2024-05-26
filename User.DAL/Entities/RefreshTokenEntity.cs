using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.DAL.Entities
{
    public class RefreshTokenEntity
    {
        public Guid Id { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime ExpiredTime { get; set; }
        public required UserEntity User { get; set; }
        public required string UserAgent { get; set; }
        public required bool isRevoked { get; set; }
    }
}
