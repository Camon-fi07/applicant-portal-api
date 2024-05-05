using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL.Entities
{
    public class ApplicationEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<ProgramPriorityEntity> Programs { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
