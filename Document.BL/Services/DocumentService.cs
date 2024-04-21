using Document.BL.Services.IServices;
using Document.DAL;
using Document.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Document.BL.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentDBContext _documentDBContext;
        public DocumentService(DocumentDBContext documentDBContext) {
            _documentDBContext = documentDBContext;
        }
        public async Task ChangeFile(IFormFile file, Guid id)
        {
            try
            {
                var fileDetails = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
                fileDetails.Name = file.FileName;

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    fileDetails.Data = stream.ToArray();
                }

                await _documentDBContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> GetFile(Guid id) {
            var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) throw new Exception();

            return file.Data;
        }

        public async Task UploadFile(IFormFile file)
        {
            try
            {
                var fileDetails = new FileDetailsEntity()
                {
                    Id = Guid.NewGuid(),
                    Name = file.FileName,
                };

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    fileDetails.Data = stream.ToArray();
                }

                _documentDBContext.FilesDetails.Add(fileDetails);
                await _documentDBContext.SaveChangesAsync();
                await DownloadFile(fileDetails.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DownloadFile(Guid id)
        {
            try
            {
                var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
                var content = new MemoryStream(file.Data);
                var path = Path.Combine(
                   Directory.GetCurrentDirectory(), "FileDownloaded",
                   file.Name);
                await CopyStream(content, path);
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
