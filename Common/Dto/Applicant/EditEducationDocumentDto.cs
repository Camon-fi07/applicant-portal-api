using Common.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Applicant
{
    public class EditEducationDocumentDto
    { 
        public string? Name { get; set; }
        public List<Guid> Files { get; set; }
    }
}
