using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Applicant
{
    public class CreateEducationDocumentDto
    {
        public string Name { get; set; }
        public Guid EducationDocumentTypeId { get; set; }
        public List<FileDto> Files { get; set; }
    }
}
