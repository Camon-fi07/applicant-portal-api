using Admission.BL.Dto;
using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Dto;
using Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Admission.BL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly AdmissionDBContext _admissionDBContext;

        public ManagerService(AdmissionDBContext admissionDBContext)
        {
            _admissionDBContext = admissionDBContext;
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

        public async Task CreateManager(Guid userId, ManagerRole role)
        {
            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception();
            }

            var manager = new ManagerEntity()
            {
                Role = role,
                User = user,
                Id = userId
            };

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

            _admissionDBContext.Managers.Remove(manager);

            await _admissionDBContext.SaveChangesAsync();
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
