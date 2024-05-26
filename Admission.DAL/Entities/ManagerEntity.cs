using Common.Enums;

namespace Admission.DAL.Entities
{
    public class ManagerEntity
    {
        public Guid Id { get; set; }
        public required UserCacheEntity User { get; set; }
        public ManagerRole Role { get; set; }
        public FacultyCacheEntity? Faculty { get; set; }
    }
}
