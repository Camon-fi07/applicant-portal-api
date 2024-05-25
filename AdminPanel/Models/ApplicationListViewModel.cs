using Common.Dto.Application;
using Common.Dto.Dictionary;
using Common.Enums;
using Common.Models;

namespace AdminPanel.Models
{
    public class ApplicationListViewModel
    {
        public required ApplicationFilterModel Filter { get; set; }
        public List<EducationProgramDto>? AvailablePrograms { get; set; }
        public List<EducationFacultyDto>? AvailableFaculties { get; set; }
        public List<UserDto>? AvailableManagers { get; set; }
        public required ApplicationsPagedListModel ApplicationsPagedListModel { get; set; }
    }
}
