namespace Applicant.DAL.Entities
{
    public class EducationDocumentTypeCacheEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required EducationLevelCacheEntity EducationLevel { get; set; }
    }
}
