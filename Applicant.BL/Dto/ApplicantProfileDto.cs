using Applicant.DAL.Entities;
using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Dto
{
    public class ApplicantProfileDto
    {
        public ProfileDto Profile { get; set; }
        public string Citizenship { get; set; }
        public PassportEntity Passport { get; set; }
        public EducationEntity Education { get; set; }
    }
}
