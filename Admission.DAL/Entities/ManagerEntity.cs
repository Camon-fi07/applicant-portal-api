using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class ManagerEntity
    {
        public Guid Id { get; set; }
        public UserEntity User { get; set; }
        public ManagerRole Role { get; set; }
        public Guid? FacultyId { get; set; }
    }
}
