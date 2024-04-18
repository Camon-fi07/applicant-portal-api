using Applicant.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.DAL
{
    public class ApplicantDBContext: DbContext
    {
        public DbSet<ApplicantEntity> Applicants { get; set; }
        public DbSet<PassportEntity> Passports { get; set; }
        public DbSet<EducationEntity> Educations { get; set; }

        public ApplicantDBContext(DbContextOptions options) : base(options)
        {

        }
    }
}
