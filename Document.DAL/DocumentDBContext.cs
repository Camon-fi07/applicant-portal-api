using Document.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.DAL
{
    public class DocumentDBContext:DbContext
    {
        public DbSet<FileDetailsEntity> FilesDetails { get; set; }

        public DocumentDBContext(DbContextOptions options) : base(options)
        {

        }
    }
}
