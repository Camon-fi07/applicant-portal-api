using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class ApplicationEntity
    {
        public Guid Id { get; set; }
        public UserEntity User { get; set; }
        public List<ProgramPriorityEntity> Programs { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EditTime { get; set; }
        public ManagerEntity? Manager { get; set; }
    }
}
