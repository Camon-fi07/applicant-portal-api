using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.DAL.Entities
{
    public class ApplicantEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Citizenship { get; set; }
        public PassportEntity? Passport { get; set; }
        public EducationEntity? Education { get; set; }
    }
}
