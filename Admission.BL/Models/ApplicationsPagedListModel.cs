using Admission.BL.Dto;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Models 
{
    public class ApplicationsPagedListModel
    {
        public List<ApplicationShortDto> Applications { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
