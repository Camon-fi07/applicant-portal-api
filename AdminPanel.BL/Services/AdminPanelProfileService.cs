using Common.Constants;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using Common.Interfaces;
using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.BL.Services
{
    public class AdminPanelProfileService:IAdminPanelProfileService
    {
        private readonly IBus _bus;
        public AdminPanelProfileService(IBus bus) {
            _bus = bus;
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
    }
}
