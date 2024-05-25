using Common.Constants;
using Common.Dto.Application;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using EasyNetQ;

namespace AdminPanel.BL.Services
{
    public class AdminPanelApplicationService : IAdminPanelApplicationService
    {
        private readonly IBus _bus;
        public AdminPanelApplicationService(IBus bus) { 
            _bus = bus;
        }

        public Task ChangeApplicationStatus(Guid applicationId, ApplicationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task EditPrograms(List<CreateProgramPriorityDto> programPriorities, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationDto> GetApplication(Guid applicationId)
        {
            return await _bus.Rpc.RequestAsync<Guid, ApplicationDto>(applicationId, x => x.WithQueueName(RabbitMQQueue.GET_APPLICATION));   
        }

        public async Task<ApplicationsPagedListModel> GetApplications(ApplicationFilterModel filterModel)
        {
            if (filterModel.Page < 1 || filterModel.Size < 1) throw new Exception();

            var applications = await _bus.Rpc.RequestAsync<ApplicationFilterModel, ApplicationsPagedListModel>(filterModel, x => x.WithQueueName(RabbitMQQueue.GET_APPLICATIONS));
            return applications;
        }

        public async Task<List<UserDto>> GetManagers(ManagerQueryModel managerQuery)
        {
            return await _bus.Rpc.RequestAsync<ManagerQueryModel, List<UserDto>>(managerQuery, x => x.WithQueueName(RabbitMQQueue.GET_MANAGERS));
        }

        public Task Refuse(Guid managerId, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public Task TakeApplication(Guid managerId, Guid applicationId)
        {
            throw new NotImplementedException();
        }
    }
}
