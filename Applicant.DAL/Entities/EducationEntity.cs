namespace Applicant.DAL.Entities
{
    public class EducationEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EducationDocumentTypeCacheEntity EducationDocument { get; set; }
        public List<Guid> Files { get; set; }
    }
}
