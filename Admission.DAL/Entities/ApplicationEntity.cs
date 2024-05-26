using Common.Enums;

namespace Admission.DAL.Entities
{
    public class ApplicationEntity
    {
        public Guid Id { get; set; }
        public required UserCacheEntity User { get; set; }
        public required EducationDocumentTypeCacheEntity? ApplicantEducationDocumentType { get; set; }
        public required List<ProgramPriorityEntity> Programs { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EditTime { get; set; }
        public ManagerEntity? Manager { get; set; }
    }
}
