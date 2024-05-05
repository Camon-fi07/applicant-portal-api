using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class ManagerEntity
    {
        [Key]
        public Guid UserId { get; set; }
        public ApplicationEntity? Application { get; set; }
        public ManagerRole Role { get; set; }
        public Guid? FacultyId { get; set; }
    }
}
