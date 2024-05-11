using Common.Dto.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.RabbitMQ
{
    public class EditApplicantDataRabbitDto
    {
        public Guid UserId { get; set; }
        public EditApplicantDataDto EditApplicantDataDto { get; set; }
    }
}
