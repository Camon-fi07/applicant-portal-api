using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.DAL.Entities
{
    public class FileEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
