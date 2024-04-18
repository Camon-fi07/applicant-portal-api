namespace Dictionary.DAL.Entities
{
    public class FacultyEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public List<EducationProgramEntity> EducationPrograms { get; set; }
    }
}
