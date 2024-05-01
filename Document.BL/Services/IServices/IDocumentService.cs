using Common.Dto;
using Common.Dto.RabbitMQ;
using Document.BL.Dto;
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
        public Task<Guid> UploadFile(UploadFileDto uploadFileDto);
        public Task<List<Guid>> UploadFiles(List<UploadFileDto> uploadFilesDto);
        public Task<FileResponseDto> GetFile(Guid id);
        public Task<List<FileResponseDto>> GetFiles(List<Guid> ids);
        public Task DeleteFile(Guid id);
        public Task DeleteFiles(List<Guid> ids);
    }
}
