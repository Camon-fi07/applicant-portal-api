using Admission.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Admission.DAL
{
    public class AdmissionDBContext:DbContext
    {
        public DbSet<ManagerEntity> Managers { get; set; }
        public DbSet<ApplicationEntity> Applications { get; set; }
        public DbSet<ProgramPriorityEntity> ProgramPriorities { get; set; }
        public DbSet<UserCacheEntity> Users { get; set; }
        public DbSet<ProgramCacheEntity> Programs { get; set; }
        public DbSet<EducationDocumentTypeCacheEntity> EducationDocumentTypes { get; set; }
        public DbSet<EducationLevelCacheEntity> EducationLevels { get; set; }
        public DbSet<FacultyCacheEntity> Faculties { get; set; }

        public AdmissionDBContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EducationLevelCacheEntity>().HasKey(l => l.Id);
            modelBuilder.Entity<EducationLevelCacheEntity>().Property(l => l.Id).ValueGeneratedNever();

            modelBuilder.Entity<EducationDocumentTypeCacheEntity>()
               .HasMany(d => d.NextEducationLevels)
               .WithMany(l => l.PreviousEducationLevelDocumentTypes)
               .UsingEntity(j => j.ToTable("LevelDocumentType"));

            modelBuilder.Entity<EducationLevelCacheEntity>()
               .HasMany(l => l.CurrentEducationLevelDocumentTypes)
               .WithOne(d => d.EducationLevel);

            modelBuilder.Entity<EducationLevelCacheEntity>()
               .HasMany(l => l.Programs)
               .WithOne(p => p.EducationLevel);

            modelBuilder.Entity<FacultyCacheEntity>()
                .HasMany(f => f.Programs)
                .WithOne(p => p.Faculty);
        }
    }
}
