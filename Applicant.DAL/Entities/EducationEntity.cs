using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.DAL.Entities
{
    public class EducationEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid EducationDocumentTypeId { get; set; }
        public List<Guid> Files { get; set; }
    }
}
