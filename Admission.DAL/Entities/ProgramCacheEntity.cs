namespace Admission.DAL.Entities
{
    public class ProgramCacheEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Code { get; set; }
        public required EducationLevelCacheEntity EducationLevel { get; set; }
        public FacultyCacheEntity Faculty { get; set; }
    }
}
