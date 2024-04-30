using Common.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Dto
{
    public class EditEducationDocumentDto
    { 
        public string? Name { get; set; }
        public Guid? EducationDocumentTypeId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
