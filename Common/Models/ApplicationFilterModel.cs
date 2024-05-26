using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ApplicationFilterModel
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string? Name { get; set; }
        public Guid? ProgramId { get; set; }
        public List<Guid>? Faculties { get; set; }
        public ApplicationStatus? Status { get; set; }
        public bool? HaveManager { get; set; }
        public Guid? ManagerId { get; set; }
        public ApplicationsSortType? SortType { get; set; }
    }
}
