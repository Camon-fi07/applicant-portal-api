namespace Dictionary.DAL.Entities
{
    public class EducationLevelEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EducationProgramEntity> EducationPrograms { get; set; }
        public List<EducationDocumentTypeEntity> CurrentEducationLevelDocumentTypes { get; set; }
        public List<EducationDocumentTypeEntity> PreviousEducationLevelDocumentTypes { get; set; }
    }
}
