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
        public required string Name { get; set; }
        public required string EducationName { get; set; }
        public required Guid EducationDocumentTypeId { get; set; }
        public required int EducationLevelId { get; set; }
        public required string EducationLevelName { get; set; }
        public required List<Guid> Files { get; set; }
    }
}
