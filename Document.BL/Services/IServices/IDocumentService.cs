using Common.Dto;
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
        public Task<Guid> UploadFile(IFormFile file);
        public Task<List<Guid>> UploadFiles(List<IFormFile> files);
        public Task ChangeFile(IFormFile file, Guid id);
        public Task<FileResponseDto> GetFile(Guid id);
        public Task DeleteFile(Guid id);
        public Task DeleteFiles(List<Guid> ids);
    }
}
