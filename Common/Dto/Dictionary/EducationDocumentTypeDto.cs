namespace Common.Dto.Dictionary
{
    public class EducationDocumentTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public EducationLevelDto EducationLevel { get; set; }
        public List<EducationLevelDto>? NextEducationLevels { get; set; }
    }
}
