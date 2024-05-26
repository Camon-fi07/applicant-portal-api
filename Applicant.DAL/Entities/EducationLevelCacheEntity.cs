namespace Applicant.DAL.Entities
{
    public class EducationLevelCacheEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<EducationDocumentTypeCacheEntity>? CurrentEducationLevelDocumentTypes { get; set; }
    }
}