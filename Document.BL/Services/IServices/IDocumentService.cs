using Common.Dto;
using Common.Dto.RabbitMQ;
using Document.BL.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.BL.Services.IServices
{
    public interface IDocumentService
    {
        public Task<FileDto> UploadFile(UploadFileDto uploadFileDto);
        public Task<FileStreamResult> GetFile(Guid id);
        public Task DeleteFile(Guid id);
        public Task DeleteFiles(List<Guid> ids);
    }
}
