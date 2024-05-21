using Common.Enums;
using Common.Models;

namespace AdminPanel.Models
{
    public class ApplicationListViewModel
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string? Name { get; set; }
        public Guid? ProgramId { get; set; }
        public List<Guid>? Faculties { get; set; }
        public ApplicationStatus Status { get; set; }
        public bool HaveManager { get; set; }
        public Guid ManagerId { get; set; }
        public ApplicationsSortType? SortType { get; set; }
        public required ApplicationsPagedListModel ApplicationsPagedListModel { get; set; }
    }
}
