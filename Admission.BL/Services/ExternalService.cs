using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Constants;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using Common.Models;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace Admission.BL.Services
{
    public class ExternalService : IExternalService
    {
        private readonly IBus _bus;
        private readonly AdmissionDBContext _admissionDBContext;
        public ExternalService(IBus bus, AdmissionDBContext admissionDBContext)
        {
            _bus = bus;
            _admissionDBContext = admissionDBContext;
        }

        public async Task<ProgramCacheEntity> GetProgram(Guid programId)
        {
            var program = await _admissionDBContext.Programs.Include(p => p.EducationLevel).FirstOrDefaultAsync(p => p.Id == programId);

            if (program == null)
            {
                await ImportProgram(programId);
                program = await _admissionDBContext.Programs.Include(p => p.EducationLevel).FirstOrDefaultAsync(p => p.Id == programId);
                if (program == null) throw new CustomExceptionModel(ErrorMessages.PROGRAM_NOT_FOUND, 400);
            }

            return program;
        }

        public async Task ImportProgram(Guid programId)
        {
            var externalProgram = await _bus.Rpc.RequestAsync<Guid, AddProgramDto>(programId, x => x.WithQueueName(RabbitMQQueue.ADD_PROGRAM));

            if (externalProgram == null) throw new CustomExceptionModel(ErrorMessages.PROGRAM_NOT_FOUND, 400);

            var educationLevel = await _admissionDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == externalProgram.EducationLevel.Id);
            if (educationLevel != null) educationLevel.Name = externalProgram.EducationLevel.Name;
            else
            {
                educationLevel = new EducationLevelCacheEntity()
                {
                    Name = externalProgram.EducationLevel.Name,
                    Id = externalProgram.EducationLevel.Id
                };
            }

            var faculty = await _admissionDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == externalProgram.Faculty.Id);
            if (faculty != null) faculty.Name = externalProgram.Faculty.Name;
            else
            {
                faculty = new FacultyCacheEntity()
                {
                    Name = externalProgram.Faculty.Name,
                    Id = externalProgram.Faculty.Id,
                };
            }

            var program = new ProgramCacheEntity()
            {
                Code = externalProgram.Code,
                EducationLevel = educationLevel,
                Name = externalProgram.Name,
                Faculty = faculty,
                Id = externalProgram.Id
            };

            _admissionDBContext.Programs.Add(program);
            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task<UserCacheEntity> GetUser(Guid userId)
        {
            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                await ImportUser(userId);
                user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) throw new CustomExceptionModel(ErrorMessages.USER_NOT_FOUND, 400);
            }

            return user;
        }

        public async Task ImportUser(Guid userId)
        {
            var externalUser = await _bus.Rpc.RequestAsync<Guid, AddUserDto>(userId, x => x.WithQueueName(RabbitMQQueue.ADD_USER));

            if (externalUser == null) throw new CustomExceptionModel(ErrorMessages.USER_NOT_FOUND, 400);

            var user = new UserCacheEntity()
            {
                Email = externalUser.Email,
                Firstname = externalUser.Firstname,
                Id = externalUser.Id,
                Lastname = externalUser.Lastname,
                Middlename = externalUser.Middlename,
            };

            _admissionDBContext.Users.Add(user);
            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task<EducationDocumentTypeCacheEntity> GetEducationDocument(Guid documentId)
        {
            var documentType = await _admissionDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).Include(d => d.NextEducationLevels).FirstOrDefaultAsync(d => d.Id == documentId);

            if (documentType == null)
            {
                await ImportEducationDocument(documentId);
                documentType = await _admissionDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).Include(d => d.NextEducationLevels).FirstOrDefaultAsync(u => u.Id == documentId);

                if (documentType == null) throw new CustomExceptionModel(ErrorMessages.EDUCATION_DOCUMENT_TYPE_NOT_FOUND, 400);
            }

            return documentType;
        }

        public async Task ImportEducationDocument(Guid documentId)
        {
            var externalDocumentType = await _bus.Rpc.RequestAsync<Guid, AddEducationDocumentTypeDto>(documentId, x => x.WithQueueName(RabbitMQQueue.ADD_EDUCATION_DOCUMENT_TYPE));

            if (externalDocumentType == null) throw new CustomExceptionModel(ErrorMessages.EDUCATION_DOCUMENT_TYPE_NOT_FOUND, 400);

            var educationLevel = await _admissionDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == externalDocumentType.EducationLevel.Id);
            if (educationLevel != null) educationLevel.Name = externalDocumentType.EducationLevel.Name;
            else
            {
                educationLevel = new EducationLevelCacheEntity()
                {
                    Name = externalDocumentType.EducationLevel.Name,
                    Id = externalDocumentType.EducationLevel.Id,
                };
            }

            List<EducationLevelCacheEntity>? nextEducationLevels = null;

            if (externalDocumentType.NextEducationLevels != null)
            {
                nextEducationLevels = new List<EducationLevelCacheEntity>();
                foreach (var el in externalDocumentType.NextEducationLevels)
                {
                    var nextEducationLevel = await _admissionDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == el.Id);
                    if (nextEducationLevel != null) nextEducationLevel.Name = el.Name;
                    else
                    {
                        nextEducationLevels.Add(new EducationLevelCacheEntity()
                        {
                            Name = el.Name,
                            Id = el.Id
                        });
                    }
                }
            }

            var documentType = new EducationDocumentTypeCacheEntity()
            {
                Name = externalDocumentType.Name,
                Id = externalDocumentType.Id,
                EducationLevel = educationLevel,
                NextEducationLevels = nextEducationLevels,
            };

            _admissionDBContext.EducationDocumentTypes.Add(documentType);
            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task<FacultyCacheEntity> GetFaculty(Guid facultyId)
        {
            var faculty = await _admissionDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == facultyId);

            if (faculty == null)
            {
                await ImportFaculty(facultyId);
                faculty = await _admissionDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == facultyId); ;
                if (faculty == null) throw new CustomExceptionModel(ErrorMessages.FACULTY_NOT_FOUND, 400);
            }

            return faculty;
        }

        public async Task ImportFaculty(Guid facultyId)
        {
            var externalFaculty = await _bus.Rpc.RequestAsync<Guid, AddFacultyDto>(facultyId, x => x.WithQueueName(RabbitMQQueue.ADD_FACULTY));

            if (externalFaculty == null) throw new CustomExceptionModel(ErrorMessages.FACULTY_NOT_FOUND, 400);

            var faculty = new FacultyCacheEntity()
            {
                Id = externalFaculty.Id,
                Name = externalFaculty.Name
            };

            _admissionDBContext.Faculties.Add(faculty);
            await _admissionDBContext.SaveChangesAsync();
        }
    }  
}
