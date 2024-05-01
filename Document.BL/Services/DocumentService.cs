using Common.Dto;
using Document.BL.Dto;
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

        public async Task<FileResponseDto> GetFile(Guid id) {
            var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) throw new Exception();

            var data = File.ReadAllBytes(file.Path);

            return new FileResponseDto()
            {
                Name = file.Name,
                Id = file.Id,
                Data = data
            };
        }

        public async Task<List<FileResponseDto>> GetFiles(List<Guid> ids)
        {
            var files = new List<FileResponseDto>();
            foreach (var fileId in ids)
            {
                files.Add(await GetFile(fileId));
            }

            return files;
        }

        public async Task<Guid> UploadFile(UploadFileDto uploadFilesDto)
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

                _documentDBContext.SaveChangesAsync();

                return fileDetails.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Guid>> UploadFiles(List<UploadFileDto> uploadFilesDto)
        {
            var filesIds = new List<Guid>();
            foreach (var file in uploadFilesDto)
            {
                filesIds.Add(await UploadFile(file));
            }

            return filesIds;
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
