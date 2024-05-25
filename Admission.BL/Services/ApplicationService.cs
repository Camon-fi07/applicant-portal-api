using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Dto;
using Common.Dto.Applicant;
using Common.Dto.Application;
using Common.Enums;
using Common.Models;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Admission.BL.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AdmissionDBContext _admissionDBContext;
        private readonly IExternalService _externalService;
        private readonly IBus _bus;
        private readonly int maxProgramsCount;

        public ApplicationService(AdmissionDBContext admissionDBContext, IBus bus, IConfiguration configuration, IExternalService externalService) {
            _admissionDBContext = admissionDBContext;
            _bus = bus;
            _externalService = externalService;
            if (!int.TryParse(configuration.GetSection("MaxProgramsCount").Value, out maxProgramsCount))
            {
                maxProgramsCount = 5;
            }
        }

        public async Task ChangeApplicationStatus(Guid applicationId, ApplicationStatus status)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).Include(a => a.User).FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) { throw new Exception(); }

            application.Status = status;

            await _admissionDBContext.SaveChangesAsync();

            await _bus.PubSub.PublishAsync(new EmailMessageDto()
            {
                Body = $"Статус вашего поступления был изменён на {status}",
                Email = application.User.Email,
                Subject = "Статус поступления"
            });
        }

        public async Task CreateApplication(CreateApplicationDto createApplication, Guid userId)
        {
            var educationDocumentType = await _externalService.GetEducationDocument(createApplication.Education.EducationDocumentTypeId);

            var programs = new List<ProgramPriorityEntity>();
            foreach (var programPrioritet in createApplication.Programs)
            {
                var programCacheEntity = await _externalService.GetProgram(programPrioritet.ProgramId);
                programs.Add(new ProgramPriorityEntity()
                {
                    Id = Guid.NewGuid(),
                    Priority = programPrioritet.Priority,
                    ProgramId = programPrioritet.ProgramId,
                    Program = programCacheEntity,
                });
            }

            var isProgramsValid = IsProgramsValid(programs, educationDocumentType);
            if (!isProgramsValid) throw new Exception();

            if(createApplication.Passport.Series.ToString().Length != 4)
            {
                throw new Exception();
            }

            if (createApplication.Passport.Number.ToString().Length != 6)
            {
                throw new Exception();
            }

            var user = await _externalService.GetUser(userId);

            ApplicationEntity applicationEntity = new ApplicationEntity() {
                Id = Guid.NewGuid(),
                Status = ApplicationStatus.Created,
                User = user,
                CreateTime = DateTime.UtcNow,
                EditTime = DateTime.UtcNow,
                ApplicantEducationDocumentType = educationDocumentType,
                Programs = programs
            };

            applicationEntity.Programs = programs;

            await _bus.PubSub.PublishAsync(new RegisterApplicantDto()
            {
                ApplicationId = applicationEntity.Id,
                Citizenship = createApplication.Citizenship,
                Education = createApplication.Education,
                Passport = createApplication.Passport,
                UserId = userId
            });

            await _bus.PubSub.PublishAsync(new EmailMessageDto()
            {
                Body = $"Ваше заявление было успешно обработано",
                Email = user.Email,
                Subject = "Поступления"
            });

            await _admissionDBContext.Applications.AddAsync(applicationEntity);

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task EditPrograms(List<CreateProgramPriorityDto> programPriorities, Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.
                Include(a => a.Programs).ThenInclude(p => p.Program).ThenInclude(p => p.EducationLevel).
                Include(a => a.ApplicantEducationDocumentType).ThenInclude(d => d.EducationLevel).
                Include(a => a.ApplicantEducationDocumentType).ThenInclude(d => d.NextEducationLevels).
                FirstOrDefaultAsync(a => a.Id == applicationId);

            if(application == null) { throw new Exception(); }

            foreach(var originProgram in application.Programs) {
                var newProgram = programPriorities.FirstOrDefault(p => p.ProgramId == originProgram.ProgramId);
                if (newProgram == null)
                {
                    _admissionDBContext.ProgramPriorities.Remove(originProgram);
                }
                else
                {
                    originProgram.Priority = newProgram.Priority;
                }
            }

            foreach (var programPrioritet in programPriorities)
            {
                var oldProgram = application.Programs.FirstOrDefault(p => p.ProgramId == programPrioritet.ProgramId);
                if (oldProgram != null) continue;

                var programCacheEntity = await _externalService.GetProgram(programPrioritet.ProgramId);
                application.Programs.Add(new ProgramPriorityEntity()
                {
                    Id = Guid.NewGuid(),
                    Priority = programPrioritet.Priority,
                    ProgramId = programPrioritet.ProgramId,
                    Program = programCacheEntity,
                });
            }

            var isProgramsValid = IsProgramsValid(application.Programs, application.ApplicantEducationDocumentType);
            if(!isProgramsValid) throw new Exception();

            application.EditTime = DateTime.UtcNow;

            await _admissionDBContext.SaveChangesAsync();
        }

        private bool IsProgramsValid(List<ProgramPriorityEntity> programPriorities, EducationDocumentTypeCacheEntity educationDocumentType)
        {
            if (programPriorities.Count > maxProgramsCount || programPriorities.Count < 1)
            {
                return false;
            }
            var uniquePriorities = new List<ProgramPriorityEntity>();

            foreach (var programPrioritet in programPriorities)
            {
                var isPriorityUnique = uniquePriorities.All(p => p.Priority != programPrioritet.Priority);
                var isNextLevel = educationDocumentType.NextEducationLevels != null && educationDocumentType.NextEducationLevels.Any(l => l.Id == programPrioritet.Program.EducationLevel.Id);
                var isCurrentLevel = educationDocumentType.EducationLevel.Id == programPrioritet.Program.EducationLevel.Id;

                if (isPriorityUnique && (isNextLevel || isCurrentLevel))
                {
                    uniquePriorities.Add(programPrioritet);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<ApplicationDto> GetApplication(Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).ThenInclude(p => p.Program).ThenInclude(p => p.Faculty).Include(a => a.User).Include(a => a.Manager).ThenInclude(m => m.User).FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null) { throw new Exception(); }

            var programPriorities = new List<ProgramPriorityDto>();

            foreach (var programPriority in application.Programs)
            {
                var program = await _externalService.GetProgram(programPriority.ProgramId);

                programPriorities.Add(new ProgramPriorityDto()
                {
                    Priority = programPriority.Priority,
                    ProgramId = programPriority.ProgramId,
                    FacultyName = programPriority.Program.Faculty.Name,
                    FacultyId = programPriority.Program.Faculty.Id,
                    ProgramName = program.Name,
                    EducationLevelName = program.EducationLevel.Name,
                    ProgramCode = program.Code
                });
            }

            UserDto? manager = null;
            if (application.Manager != null)
            {
                manager = new UserDto()
                {
                    Email = application.Manager.User.Email,
                    Firstname = application.Manager.User.Firstname,
                    Id = application.Manager.User.Id,
                    Lastname = application.Manager.User.Lastname,
                    Middlename = application.Manager.User.Middlename,
                };
            }

            return new ApplicationDto()
            {
                ApplicationId = applicationId,
                Status = application.Status,
                ProgramPriorities = programPriorities,
                User = new UserDto()
                {
                    Email = application.User.Email,
                    Firstname = application.User.Firstname,
                    Id = application.User.Id,
                    Lastname = application.User.Lastname,
                    Middlename = application.User.Middlename
                },
                CreateTime = application.CreateTime,
                EditTime = application.EditTime,
                Manager = manager
            };
        }

        public async Task<ApplicationsPagedListModel> GetApplications(int page, int size, string? name, Guid? programId, List<Guid>? faculties, ApplicationStatus? status, bool? haveManager, Guid? managerId, ApplicationsSortType? sortType)
        {
            var applications = _admissionDBContext.Applications.
                Include(a => a.Programs).ThenInclude(p => p.Program).ThenInclude(p => p.Faculty).
                Include(p => p.Programs).ThenInclude(p => p.Program).ThenInclude(p => p.EducationLevel).
                Include(a => a.User).Include(a => a.Manager).ThenInclude(m => m.User).ToList();

            if (programId != null) applications = applications.Where(a => a.Programs.Any(p => p.ProgramId == programId)).ToList();
            if (faculties != null && faculties.Count > 0) applications = applications.Where(a => a.Programs.Any((p) => faculties.Contains(p.Program.Faculty.Id))).ToList();

            if (status != null) applications = applications.Where(a => a.Status == status).ToList();

            if (name != null) applications = applications.Where(a => a.User.Lastname.ToLower().Contains(name.ToLower())).ToList();
            if (managerId != null) applications = applications.Where(a => a.Manager?.Id ==  managerId).ToList();
            if (haveManager != null) applications = applications.Where(a => (a.Manager == null && !(bool)haveManager) || (a.Manager != null && (bool)haveManager)).ToList();

            if (sortType != null) applications = SortApplicationDate((ApplicationsSortType)sortType, applications);
            int count = applications.Count() / (page * size) == 0 ? 1 : applications.Count() / (page * size);
            applications = applications.Skip((page - 1) * size).Take(size).ToList();

            var applicationsResult = applications.Select(a => {

                UserDto? manager = null;
                if (a.Manager != null)
                {
                    manager = new UserDto()
                    {
                        Email = a.Manager.User.Email,
                        Firstname = a.Manager.User.Firstname,
                        Id = a.Manager.User.Id,
                        Lastname = a.Manager.User.Lastname,
                        Middlename = a.Manager.User.Middlename,
                    };
                }

                return new ApplicationShortDto()
                {
                    ApplicationId = a.Id,
                    ProgramPriorities = a.Programs.Select(p => new ProgramPriorityDto()
                    {
                        FacultyId = p.Program.Faculty.Id,
                        Priority = p.Priority,
                        ProgramId = p.ProgramId,
                        FacultyName = p.Program.Faculty.Name,
                        ProgramName = p.Program.Name,
                        EducationLevelName = p.Program.EducationLevel.Name,
                        ProgramCode = p.Program.Code
                    }).ToList(),
                    Status = a.Status,
                    User = new UserDto()
                    {
                        Email = a.User.Email,
                        Firstname = a.User.Firstname,
                        Id = a.User.Id,
                        Lastname = a.User.Lastname,
                        Middlename = a.User.Middlename
                    },
                    Manager = manager
                };
            }).ToList();

            return new ApplicationsPagedListModel()
            {
                Applications = applicationsResult,
                Pagination = new PageInfoModel()
                {
                    Count = count,
                    Current = page,
                    Size = size
                }
            };
        }

        public async Task ReturnApplication(Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null) { throw new Exception(); }

            application.Manager = null;
            application.EditTime = DateTime.UtcNow;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task TakeApplication(Guid managerId, Guid applicationId)
        {
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);
            if(manager == null) { throw new Exception(); }

            var application = await _admissionDBContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);

            var user = await _externalService.GetUser(managerId);

            if (application == null) { throw new Exception(); }

            application.Manager = manager;
            application.EditTime = DateTime.UtcNow;

            await _admissionDBContext.SaveChangesAsync();

            await _bus.PubSub.PublishAsync(new EmailMessageDto()
            {
                Body = $"Ваше заявление обрабатывается менеджером {user.Lastname} {user.Firstname} {user.Middlename}",
                Email = user.Email,
                Subject = "Заявление на поступления"
            });

            await _bus.PubSub.PublishAsync(new EmailMessageDto()
            {
                Body = $"Вы были назначены менеджером {user.Lastname} {user.Firstname} {user.Middlename}",
                Email = user.Email,
                Subject = "Заявление на поступления"
            });
        }

        private List<ApplicationEntity> SortApplicationDate(ApplicationsSortType sortType, List<ApplicationEntity> applications)
        {
            switch (sortType)
            {
                case ApplicationsSortType.CreateAsc:
                    return applications.OrderBy(a => a.CreateTime).ToList();
                case ApplicationsSortType.CreateDesc:
                    return applications.OrderByDescending(a => a.CreateTime).ToList();
                case ApplicationsSortType.EditAsc:
                    return applications.OrderBy(a => a.EditTime).ToList();
                case ApplicationsSortType.EditDesc:
                    return applications.OrderByDescending(a => a.EditTime).ToList();
                default:
                    return applications;
            }
        }
    }
}
