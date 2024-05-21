using Common.Dto.Manager;

namespace Admission.BL.Services.IServices
{
    public interface IManagerService
    {
        public Task CreateManager(CreateManagerDto createManagerDto);
        public Task DeleteManager(Guid managerId);
        public Task AddManagerToFaculty(Guid managerId, Guid facultyId);
        public Task DeleteManagerFromFaculty(Guid managerId);
    }
}
