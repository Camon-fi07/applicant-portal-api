using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class EditUserProfileDto
    {
        public Guid UserId { get; set; }
        public EditProfileDto EditProfileDto { get; set; }
    }
}
