using Common.Constants;
using Common.Dto.Applicant;
using Common.Dto.RabbitMQ;
using Common.Interfaces;
using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.BL.Services
{
    public class AdminPanelApplicantService : IAdminPanelApplicantService
    {
        private readonly IBus _bus;
        public AdminPanelApplicantService(IBus bus) { 
            _bus = bus;
        }

        public async Task<ApplicantDataDto> EditApplicantData(EditApplicantDataDto editApplicantDataDto, Guid userId)
        {
            EditApplicantDataRabbitDto editApplicantDataRabbitDto = new EditApplicantDataRabbitDto() { 
                EditApplicantDataDto = editApplicantDataDto,
                UserId = userId
            };

            return await _bus.Rpc.RequestAsync<EditApplicantDataRabbitDto, ApplicantDataDto>(editApplicantDataRabbitDto, x => x.WithQueueName(RabbitMQQueue.EDIT_APPLICANT_DATA));
        }

        public async Task<ApplicantDataDto> GetApplicantData(Guid userId)
        {
            return await _bus.Rpc.RequestAsync<Guid, ApplicantDataDto>(userId, x => x.WithQueueName(RabbitMQQueue.GET_APPLICANT_DATA));
        }
    }
}
