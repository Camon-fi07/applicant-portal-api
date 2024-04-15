using Common.Models;
using Dictionary.BL.Dto;

namespace Dictionary.BL.Models
{
    public class ProgramPagedListModel
    {
        public List<EducationProgramDto> Programs { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
