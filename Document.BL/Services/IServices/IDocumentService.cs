using Common.Dto;
using Common.Dto.RabbitMQ;
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
        public Task<FileResponseDto> UploadFile(IFormFile file);
        public Task<List<FileResponseDto>> UploadFiles(List<IFormFile> files);
        public Task<FileResponseDto> ChangeFile(ChangeFileDto changeFileDto);
        public Task<List<FileResponseDto>> ChangeFiles(List<ChangeFileDto> changeFilesDto);
        public Task<FileResponseDto> GetFile(Guid id);
        public Task<List<FileResponseDto>> GetFiles(List<Guid> ids);
        public Task DeleteFile(Guid id);
        public Task DeleteFiles(List<Guid> ids);
    }
}
