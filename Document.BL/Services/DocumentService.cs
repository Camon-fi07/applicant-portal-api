using Common.Dto;
using Common.Models;
using Document.BL.Dto;
using Document.BL.Services.IServices;
using Document.DAL;
using Document.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Document.BL.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentDBContext _documentDBContext;
        public DocumentService(DocumentDBContext documentDBContext) {
            _documentDBContext = documentDBContext;
        }

        public async Task<FileStreamResult> GetFile(Guid id)
        {
            var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) throw new CustomExceptionModel("Файл не найден", 400);

            var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(fileStream, "application/octet-stream")
            {
                FileDownloadName = file.Name,
            };
        }

        public async Task<FileDto> UploadFile(UploadFileDto uploadFilesDto)
        {
            try
            {
                var fileDetails = new FileDetailsEntity()
                {
                    Id = Guid.NewGuid(),
                    Name = uploadFilesDto.File.FileName,
                };

                var data = TranslateFileToByte(uploadFilesDto.File);
                
                await _documentDBContext.SaveChangesAsync();
                fileDetails.Path = await SaveFile(data, fileDetails.Name);

                _documentDBContext.FilesDetails.Add(fileDetails);

                await _documentDBContext.SaveChangesAsync();

                return new FileDto()
                {
                    Name = fileDetails.Name,
                    Id = fileDetails.Id,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteFile(Guid id)
        {
            try
            {
                var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
                var path = Path.Combine(
                   Directory.GetCurrentDirectory(), "FileDownloaded",
                   file.Name);

                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteFiles(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                await DeleteFile(id);
            }
        }

        private byte[] TranslateFileToByte(IFormFile formFile)
        {
            using var stream = new MemoryStream();
            formFile.CopyTo(stream);
            return stream.ToArray();
        }

        private async Task<string> SaveFile(byte[] data, string name)
        {
            try
            {
                var content = new MemoryStream(data);
                var path = Path.Combine(
                   Directory.GetCurrentDirectory(), "FileDownloaded", name);
                await CopyStream(content, path);
                return path;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
