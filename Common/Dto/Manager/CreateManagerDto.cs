using Common.Enums;

namespace Common.Dto.Manager
{
    public class CreateManagerDto
    {
        public Guid UserId { get; set; }
        public ManagerRole Role { get; set; }
        public Guid? FacultyId { get; set; }
    }
}
