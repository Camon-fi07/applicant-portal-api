using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.DAL.Entities
{
    public class FileDetailsEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
