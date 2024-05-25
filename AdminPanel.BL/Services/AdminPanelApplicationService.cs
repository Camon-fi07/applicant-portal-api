using Common.Constants;
using Common.Dto;
using Common.Dto.Application;
using Common.Dto.RabbitMQ;
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

        public async Task ChangeApplicationStatus(Guid applicationId, ApplicationStatus status)
        {
            ChangeStatusDto changeStatus = new ChangeStatusDto()
            {
                ApplicationId = applicationId,
                Status = status
            };
            await _bus.Rpc.RequestAsync<ChangeStatusDto, Empty>(changeStatus, x => x.WithQueueName(RabbitMQQueue.CHANGE_APPLICATION_STATUS));
        }

        public async Task EditPrograms(List<CreateProgramPriorityDto> programPriorities, Guid applicationId)
        {
            EditProgramPriorityDto editProgramPriorityDto = new EditProgramPriorityDto() { 
                ApplicationId = applicationId,
                Programs = programPriorities
            };
            await _bus.Rpc.RequestAsync<EditProgramPriorityDto, Empty>(editProgramPriorityDto, x => x.WithQueueName(RabbitMQQueue.EDIT_PROGRAMS_PRIORITY));
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

        public async Task ReturnApplication(Guid applicationId)
        {
            await _bus.Rpc.RequestAsync<Guid, Empty>(applicationId, x => x.WithQueueName(RabbitMQQueue.RETURN_APPLICATION));
        }

        public async Task SetManager(Guid managerId, Guid applicationId)
        {
            SetManagerApplicationDto setManagerApplication = new SetManagerApplicationDto()
            {
                ApplicationId = applicationId,
                ManagerId = managerId
            };

            await _bus.Rpc.RequestAsync<SetManagerApplicationDto, Empty>(setManagerApplication, x => x.WithQueueName(RabbitMQQueue.SET_MANAGER));

        }
    }
}
