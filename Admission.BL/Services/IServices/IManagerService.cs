using Admission.BL.Dto;
using Admission.DAL.Entities;
using Common.Dto;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Services.IServices
{
    public interface IManagerService
    {
        public Task CreateManager(Guid userId, ManagerRole role);
        public Task DeleteManager(Guid managerId);
        public Task AddUser(AddUserDto addUser);
        public Task EditUser(EditUserDto editUser);
        public Task AddManagerToFaculty(Guid managerId, Guid facultyId);
        public Task DeleteManagerFromFaculty(Guid managerId);
    }
}
