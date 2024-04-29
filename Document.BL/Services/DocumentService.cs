using Common.Dto;
using Document.BL.Services.IServices;
using Document.DAL;
using Document.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

                await DeleteFile(id);
                await _documentDBContext.SaveChangesAsync();
                await DownloadFile(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FileResponseDto> GetFile(Guid id) {
            var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) throw new Exception();

            return new FileResponseDto()
            {
                Data = file.Data,
                Name = file.Name
            };
        }

        public async Task<Guid> UploadFile(IFormFile file)
        {
            return await UploadSingleFile(file);
        }

        public async Task<List<Guid>> UploadFiles(List<IFormFile> files)
        {
            var filesIds = new List<Guid>();
            foreach (var file in files)
            {
                filesIds.Add(await UploadSingleFile(file));
            }

            return filesIds;
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

        public async Task DeleteFile(Guid id)
        {
            await DeleteSingleFile(id);
        }

        public async Task DeleteFiles(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                await DeleteSingleFile(id);
            }
        }

        private async Task<Guid> UploadSingleFile(IFormFile file)
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

                return fileDetails.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DeleteSingleFile(Guid id)
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

        private async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
