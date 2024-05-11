using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Applicant
{
    public class EducationDocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid EducationDocumentTypeId { get; set; }
        public List<Guid> Files { get; set; }
    }
}
