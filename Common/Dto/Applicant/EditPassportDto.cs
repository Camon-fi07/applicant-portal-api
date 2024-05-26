using Common.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.Applicant
{
    public class EditPassportDto
    {
        public int? Series { get; set; }
        public int? Number { get; set; }
        public string? BirthPlace { get; set; }
        public string? Issued { get; set; }
        public List<FileDto>? Files { get; set; }
    }
}
