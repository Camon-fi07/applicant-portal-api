using Common.Dto.Application;
using Common.Enums;
using Common.Models;

namespace Common.Interfaces
{
    public interface IAdminPanelApplicationService
    {
        public Task EditPrograms(List<CreateProgramPriorityDto> programPriorities, Guid applicationId);
        public Task<ApplicationDto> GetApplication(Guid applicationId);
        public Task<ApplicationsPagedListModel> GetApplications(ApplicationFilterModel filterModel);
        public Task ChangeApplicationStatus(Guid applicationId, ApplicationStatus status);
        public Task SetManager(Guid managerId, Guid applicationId);
        public Task ReturnApplication(Guid applicationId);
        public Task<List<UserDto>> GetManagers(ManagerQueryModel managerQuery);
    }
}
