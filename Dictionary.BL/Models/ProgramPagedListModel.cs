using Common.Models;
using Dictionary.DAL.Entities;

namespace Dictionary.BL.Models
{
    public class ProgramPagedListModel
    {
        public List<EducationProgramEntity> Programs { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
