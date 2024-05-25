using Common.Dto.Application;

namespace AdminPanel.Models
{
    public class ApplicationViewModel
    {
        public ApplicationDto Application { get; set; }
        public int MaxProgramsCount { get; set; }
        public List<UserDto>? AvailableManagers { get; set; }
    }
}
