namespace Admission.DAL.Entities
{
    public class ProgramPriorityEntity
    {
        public ApplicationEntity Application { get; set; }
        public required Guid Id { get; set; }
        public required Guid ProgramId { get; set; }
        public required ProgramCacheEntity Program { get; set; }
        public required int Priority { get; set; }
    }
}
