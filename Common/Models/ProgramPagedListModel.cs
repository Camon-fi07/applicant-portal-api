using Common.Dto.Dictionary;
using Common.Models;

namespace Common.Models
{
    public class ProgramPagedListModel
    {
        public List<EducationProgramDto> Programs { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
