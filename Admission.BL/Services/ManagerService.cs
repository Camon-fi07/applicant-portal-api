using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Constants;
using Common.Dto.Manager;
using Common.Dto.RabbitMQ;
using Common.Enums;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace Admission.BL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly AdmissionDBContext _admissionDBContext;
        private readonly IExternalService _externalService;
        private readonly IBus _bus;

        public ManagerService(AdmissionDBContext admissionDBContext, IBus bus, IExternalService externalService)
        {
            _admissionDBContext = admissionDBContext;
            _bus = bus;
            _externalService = externalService;
        }

        public async Task CreateManager(CreateManagerDto createManagerDto)
        {
            var user = await _externalService.GetUser(createManagerDto.UserId);

            FacultyCacheEntity? faculty = null;
            if (createManagerDto.FacultyId != null) faculty = await _externalService.GetFaculty((Guid)createManagerDto.FacultyId);

            var manager = new ManagerEntity()
            {
                Role = createManagerDto.Role,
                User = user,
                Id = createManagerDto.UserId,
                Faculty = faculty
            };

            await _bus.PubSub.PublishAsync(new ChangeRoleDto()
            {
                Role = RoleType.Manager,
                UserId = createManagerDto.UserId
            }, RabbitMQQueue.ADD_ROLE);

            _admissionDBContext.Managers.Add(manager);

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task DeleteManager(Guid managerId)
        {
            var manager = await _admissionDBContext.Managers.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
            {
                throw new Exception();
            }

            _admissionDBContext.Managers.Remove(manager);

            var hasUserApplications = _admissionDBContext.Applications.Include(a => a.User).Any(a => a.User.Id == managerId);

            if (!hasUserApplications) _admissionDBContext.Users.Remove(manager.User);

            await _admissionDBContext.SaveChangesAsync();

            await _bus.PubSub.PublishAsync(new ChangeRoleDto()
            {
                Role = RoleType.Manager,
                UserId = managerId
            }, RabbitMQQueue.DELETE_ROLE);
        }

        public async Task AddManagerToFaculty(Guid managerId, Guid facultyId)
        {
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
            {
                throw new Exception();
            }

            var faculty = await _externalService.GetFaculty(facultyId);

            manager.Faculty = faculty;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task DeleteManagerFromFaculty(Guid managerId)
        {
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
            {
                throw new Exception();
            }

            manager.Faculty = null;

            await _admissionDBContext.SaveChangesAsync();
        }
    }
}
