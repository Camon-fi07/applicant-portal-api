using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.DAL.Entities
{
    public class TokenEntity
    {
        [Key]
        public string Token { get; set; }
    }
}
