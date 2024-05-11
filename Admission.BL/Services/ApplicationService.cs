using Admission.BL.Dto;
using Admission.BL.Enums;
using Admission.BL.Models;
using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Constants;
using Common.Dto;
using Common.Dto.Applicant;
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
        private readonly IBus _bus;
        private readonly IManagerService _managerService;
        private readonly int maxProgramsCount;

        public ApplicationService(AdmissionDBContext admissionDBContext, IBus bus, IConfiguration configuration, IManagerService managerService) {
            _admissionDBContext = admissionDBContext;
            _bus = bus;
            _managerService = managerService;
            if (!int.TryParse(configuration.GetSection("MaxProgramsCount").Value, out maxProgramsCount))
            {
                maxProgramsCount = 5;
            }
        }

        public async Task ChangeApplicationStatus(Guid applicationId, ApplicationStatus status)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) { throw new Exception(); }

            application.Status = status;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task CreateApplication(CreateApplicationDto createApplication, Guid userId)
        {
            if(createApplication.Programs.Count > maxProgramsCount || createApplication.Programs.Count < 1)
            {
                throw new Exception();
            }

            if(createApplication.Passport.Series.ToString().Length != 4)
            {
                throw new Exception();
            }

            if (createApplication.Passport.Number.ToString().Length != 6)
            {
                throw new Exception();
            }

            var uniquePriorities = new List<int>();
            createApplication.Programs.ForEach(p =>
            {
                if (uniquePriorities.Contains(p.Priority))
                {
                    throw new Exception();
                }
                else
                {
                    uniquePriorities.Add(p.Priority);
                }
            });

            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id ==  userId);

            if(user == null) {
                var createUserDto = await _bus.Rpc.RequestAsync<Guid, AddUserDto>(userId, x => x.WithQueueName(RabbitMQQueue.ADD_USER));
                await _managerService.AddUser(createUserDto);
                user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }

            ApplicationEntity applicationEntity = new ApplicationEntity() {
                Id = Guid.NewGuid(),
                Programs = createApplication.Programs.Select(p => new ProgramPriorityEntity()
                {
                    FacultyId = p.FacultyId,
                    Id = Guid.NewGuid(),
                    Priority = p.Priority,
                    ProgramId = p.ProgramId
                }).ToList(),
                Status = ApplicationStatus.Created,
                User = user
            };

            await _bus.PubSub.PublishAsync(new RegisterApplicantDto()
            {
                ApplicationId = applicationEntity.Id,
                Citizenship = createApplication.Citizenship,
                Education = createApplication.Education,
                Passport = createApplication.Passport,
                UserId = userId
            });

            await _admissionDBContext.Applications.AddAsync(applicationEntity);

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task EditPrograms(List<ProgramPriorityDto> programPriorities, Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);
            if(application == null) { throw new Exception(); }

            _admissionDBContext.ProgramPriorities.RemoveRange(application.Programs);

            await _admissionDBContext.SaveChangesAsync();

            var programsEntities = programPriorities.Select(p => new ProgramPriorityEntity()
            {
                FacultyId = p.FacultyId,
                Id = Guid.NewGuid(),
                Priority = p.Priority,
                ProgramId = p.ProgramId
            }).ToList();

            _admissionDBContext.ProgramPriorities.AddRange(programsEntities);

            await _admissionDBContext.SaveChangesAsync();

            application.Programs = programsEntities;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task<ApplicationDto> GetApplication(Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).Include(u => u.User).FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null) { throw new Exception(); }

            var applicant = await _bus.Rpc.RequestAsync<Guid, ApplicantDataDto>(application.User.Id, x=>x.WithQueueName(RabbitMQQueue.GET_APPLICANT_DATA));

            return new ApplicationDto()
            {
                ApplicationId = applicationId,
                Status = application.Status,
                ProgramPriorities = application.Programs.Select(p => new ProgramPriorityDto() { FacultyId = p.FacultyId, Priority = p.Priority, ProgramId = p.ProgramId }).ToList(),
                Applicant = applicant,
                User = new UserDto()
                {
                    Email = application.User.Email,
                    Firstname = application.User.Firstname,
                    Id = application.User.Id,
                    Lastname = application.User.Lastname,
                    Middlename = application.User.Middlename
                }
            };
        }

        public async Task<ApplicationsPagedListModel> GetApplications(int page, int size, string? name, Guid? programId, List<Guid>? faculties, ApplicationStatus? status, bool? haveManager, Guid? managerId, ApplicationsSortType? sortType)
        {
            var applications = _admissionDBContext.Applications.Include(a => a.Programs).Include(a => a.User).Include(a => a.Manager).ToList();

            if (programId != null) applications = applications.Where(a => a.Programs.Any(p => p.ProgramId == programId)).ToList();
            if (faculties != null && faculties.Count > 0) applications = applications.Where(a => a.Programs.Any(p => faculties.Contains(p.FacultyId))).ToList();
            if (status != null) applications = applications.Where(a => a.Status == status).ToList();

            if (name != null) applications = applications.Where(a => a.User.Lastname.ToLower().Contains(name.ToLower())).ToList();
            if (managerId != null) applications = applications.Where(a => a.Manager.Id ==  managerId).ToList();
            if (haveManager != null) applications = applications.Where(a => (a.Manager == null && !(bool)haveManager) || (a.Manager != null && (bool)haveManager)).ToList();
            if (sortType != null) applications = SortApplicationDate((ApplicationsSortType)sortType, applications);
            int count = applications.Count() / (page * size) == 0 ? 1 : applications.Count() / (page * size);
            applications = applications.Skip((page - 1) * size).Take(size).ToList();

            var applicationsResult = applications.Select(a => new ApplicationShortDto()
            {
                ApplicationId = a.Id,
                ProgramPriorities = a.Programs.Select(p => new ProgramPriorityDto() { FacultyId = p.FacultyId, Priority = p.Priority, ProgramId = p.ProgramId }).ToList(),
                Status = a.Status,
                User = new UserDto()
                {
                    Email = a.User.Email,
                    Firstname = a.User.Firstname,
                    Id = a.User.Id,
                    Lastname = a.User.Lastname,
                    Middlename = a.User.Middlename
                }
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

        public async Task Refuse(Guid managerId, Guid applicationId)
        {
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);
            if (manager == null) { throw new Exception(); }

            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null) { throw new Exception(); }

            application.Manager = null;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task TakeApplication(Guid managerId, Guid applicationId)
        {
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);
            if(manager == null) { throw new Exception(); }

            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null) { throw new Exception(); }

            application.Manager = manager;

            await _admissionDBContext.SaveChangesAsync();
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
