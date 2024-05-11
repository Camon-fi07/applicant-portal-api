using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Dto
{
    public class CreateManagerDto
    {
        public Guid UserId { get; set; }
        public ManagerRole Role { get; set; }
        public Guid? FacultyId { get; set; }
    }
}
