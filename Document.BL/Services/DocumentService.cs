using Common.Dto;
using Common.Dto.RabbitMQ;
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
        public async Task<FileResponseDto> ChangeFile(ChangeFileDto changeFileDto)
        {
            try
            {
                var fileDetails = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == changeFileDto.FileId);
                fileDetails.Name = changeFileDto.File.FileName;

                using (var stream = new MemoryStream())
                {
                    changeFileDto.File.CopyTo(stream);
                    fileDetails.Data = stream.ToArray();
                }

                await DeleteFile(changeFileDto.FileId);
                await _documentDBContext.SaveChangesAsync();
                await DownloadFile(changeFileDto.FileId);

                return new FileResponseDto()
                {
                    Data = fileDetails.Data,
                    Name = fileDetails.Name,
                    Id = fileDetails.Id
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FileResponseDto>> ChangeFiles(List<ChangeFileDto> changeFilesDto)
        {
            var files = new List<FileResponseDto>();
            foreach(var changeFileDto in changeFilesDto)
            {
                files.Add(await ChangeFile(changeFileDto));
            }
            return files;
        }

        public async Task<FileResponseDto> GetFile(Guid id) {
            var file = await _documentDBContext.FilesDetails.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) throw new Exception();

            return new FileResponseDto()
            {
                Data = file.Data,
                Name = file.Name,
                Id = file.Id
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

        public async Task<FileResponseDto> UploadFile(IFormFile file)
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

                return new FileResponseDto()
                {
                    Name = fileDetails.Name,
                    Data = fileDetails.Data,
                    Id = fileDetails.Id,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FileResponseDto>> UploadFiles(List<IFormFile> files)
        {
            var filesIds = new List<FileResponseDto>();
            foreach (var file in files)
            {
                filesIds.Add(await UploadFile(file));
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

        private async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
