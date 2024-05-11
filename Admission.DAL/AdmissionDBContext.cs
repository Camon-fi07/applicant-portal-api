using Admission.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Admission.DAL
{
    public class AdmissionDBContext:DbContext
    {
        public DbSet<ManagerEntity> Managers { get; set; }
        public DbSet<ApplicationEntity> Applications { get; set; }
        public DbSet<ProgramPriorityEntity> ProgramPriorities { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        public AdmissionDBContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
