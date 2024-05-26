using Common.Constants;
using Common.Dto;
using Common.Dto.Manager;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using Common.Enums;
using Common.Interfaces;
using EasyNetQ;

namespace AdminPanel.BL.Services
{
    public class AdminPanelProfileService:IAdminPanelProfileService
    {
        private readonly IBus _bus;
        public AdminPanelProfileService(IBus bus) {
            _bus = bus;
        }

        public async Task DeleteManagerRole(Guid id)
        {
            await _bus.Rpc.RequestAsync<Guid, Empty>(id, x=> x.WithQueueName(RabbitMQQueue.DELETE_MANAGER_ROLE));
        }

        public async Task SetManagerRole(Guid id, ManagerRole role)
        {
            CreateManagerDto createManager = new CreateManagerDto()
            {
                UserId = id,
                Role = role
            };
            await _bus.Rpc.RequestAsync<CreateManagerDto, Empty>(createManager, x => x.WithQueueName(RabbitMQQueue.SET_MANAGER_ROLE));
        }

        public async Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto)
        {
            var editProfile = new EditRabbitProfileDto()
            {
                EditProfileDto = editProfileDto,
                UserId = userId,
            };

            var profile = await _bus.Rpc.RequestAsync<EditRabbitProfileDto, ProfileDto>(editProfile, x => x.WithQueueName(RabbitMQQueue.EDIT_PROFILE));

            return profile;
        }

        public async Task<ProfileDto> GetProfile(Guid userId)
        {
            var profile = await _bus.Rpc.RequestAsync<Guid, ProfileDto>(userId, x => x.WithQueueName(RabbitMQQueue.GET_PROFILE));

            return profile;
        }

        public Task SetManager(ManagerRole role, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
