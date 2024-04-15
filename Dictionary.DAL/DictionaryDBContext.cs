using Dictionary.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.DAL
{
    public class DictionaryDBContext:DbContext
    {
        public DbSet<EducationLevelEntity> EducationLevels { get; set; }
        public DbSet<EducationDocumentTypeEntity> EducationDocumentTypes { get; set; }
        public DbSet<FacultyEntity> Faculties { get; set; }
        public DbSet<EducationProgramEntity> EducationPrograms { get; set; }
        public DbSet<ImportingStatusEntity> ImportingStatuses { get; set; }

        public DictionaryDBContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EducationDocumentTypeEntity>()
                .HasMany(d => d.NextEducationLevels)
                .WithMany(l => l.PreviousEducationLevelDocumentTypes)
                .UsingEntity(j => j.ToTable("LevelDocumentType"));

            modelBuilder.Entity<EducationLevelEntity>()
               .HasMany(l => l.CurrentEducationLevelDocumentTypes)
               .WithOne(d => d.EducationLevel);

            modelBuilder.Entity<EducationLevelEntity>()
               .HasMany(l => l.EducationPrograms)
               .WithOne(p => p.EducationLevel);

            modelBuilder.Entity<FacultyEntity>()
                .HasMany(f => f.EducationPrograms)
                .WithOne(p => p.Faculty);

            modelBuilder.Entity<EducationLevelEntity>().HasKey(l => l.Id);
            modelBuilder.Entity<EducationLevelEntity>().Property(l => l.Id).ValueGeneratedNever();
        }
    }
}
