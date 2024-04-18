namespace Dictionary.DAL.Entities
{
    public class EducationDocumentTypeEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public EducationLevelEntity EducationLevel { get; set; }
        public List<EducationLevelEntity>? NextEducationLevels { get; set; }
    }
}
