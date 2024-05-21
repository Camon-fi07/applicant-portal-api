using Common.Dto.Application;

namespace Common.Models
{
    public class ApplicationsPagedListModel
    {
        public List<ApplicationShortDto> Applications { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
