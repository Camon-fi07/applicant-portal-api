using Admission.BL.Dto;
using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Constants;
using Common.Dto;
using Common.Dto.Applicant;
using Common.Dto.RabbitMQ;
using Common.Enums;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace Admission.BL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly AdmissionDBContext _admissionDBContext;
        private readonly IBus _bus;

        public ManagerService(AdmissionDBContext admissionDBContext, IBus bus)
        {
            _admissionDBContext = admissionDBContext;
            _bus = bus;
        }

        public async Task AddUser(AddUserDto addUser)
        {
            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == addUser.Id);

            if(user != null)
            {
                throw new Exception();
            }

            _admissionDBContext.Users.Add(new UserEntity()
            {
                Firstname = addUser.Firstname,
                Id = addUser.Id,
                Lastname = addUser.Lastname,
                Middlename = addUser.Middlename,
                Email = addUser.Email
            });

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task EditUser(EditUserDto editUser)
        {
            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == editUser.Id);

            if (user == null)
            {
                throw new Exception();
            }

            if (editUser.Firstname != null) user.Firstname = editUser.Firstname;
            if (editUser.Lastname != null) user.Lastname = editUser.Lastname;
            if (editUser.Middlename != null) user.Middlename = editUser.Middlename;
            if (editUser.Email != null) user.Email = editUser.Email;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task CreateManager(CreateManagerDto createManagerDto)
        {
            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == createManagerDto.UserId);

            if (user == null)
            {
                throw new Exception();
            }

            var manager = new ManagerEntity()
            {
                Role = createManagerDto.Role,
                User = user,
                Id = createManagerDto.UserId,
                FacultyId = createManagerDto.FacultyId
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
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
            {
                throw new Exception();
            }

            var applications = _admissionDBContext.Applications.Include(a => a.Manager).Where(a => a.Manager.Id == managerId);

            foreach(var application in applications)
            {
                application.Manager = null;
            }

            _admissionDBContext.Managers.Remove(manager);

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

            manager.FacultyId = facultyId;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task DeleteManagerFromFaculty(Guid managerId)
        {
            var manager = await _admissionDBContext.Managers.FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
            {
                throw new Exception();
            }

            manager.FacultyId = null;

            await _admissionDBContext.SaveChangesAsync();
        }
    }
}
