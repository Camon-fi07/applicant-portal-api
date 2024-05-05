using Applicant.DAL.Entities;
using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Dto
{
    public class ApplicantProfileDto:ProfileDto
    {
        public string Citizenship { get; set; }
    }
}
