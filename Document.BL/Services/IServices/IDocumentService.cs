using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.BL.Services.IServices
{
    public interface IDocumentService
    {
        public Task UploadFile(IFormFile file);
        public Task ChangeFile(IFormFile file, Guid id);
        public Task<byte[]> GetFile(Guid Id);
    }
}
