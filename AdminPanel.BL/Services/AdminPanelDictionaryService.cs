using Common.Constants;
using Common.Dto;
using Common.Dto.Dictionary;
using Common.Interfaces;
using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.BL.Services
{
    public class AdminPanelDictionaryService : IAdminPanelDictionaryService
    {
        private readonly IBus _bus;
        public AdminPanelDictionaryService(IBus bus) {
            _bus = bus;
        }

        public async Task<List<EducationFacultyDto>> GetFaculties()
        {
            return await _bus.Rpc.RequestAsync<Empty, List<EducationFacultyDto>>(new Empty(), x => x.WithQueueName(RabbitMQQueue.GET_FACULTIES));
        }

        public async Task<List<ImportingStatusDto>> GetImportingStatuses()
        {
            return await _bus.Rpc.RequestAsync<Empty, List<ImportingStatusDto>>(new Empty(), x => x.WithQueueName(RabbitMQQueue.GET_IMPORTING_STATUSES));
        }

        public async Task<List<EducationProgramDto>> GetPrograms()
        {
            return await _bus.Rpc.RequestAsync<Empty, List<EducationProgramDto>>(new Empty(), x => x.WithQueueName(RabbitMQQueue.GET_ALL_PROGRAMS));
        }

        public async Task ImportEducationDocuments()
        {
            await _bus.PubSub.PublishAsync(new Empty(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_DOCUMENTS));
        }

        public async Task ImportEducationLevels()
        {
            await _bus.PubSub.PublishAsync(new Empty(), x => x.WithTopic(RabbitMQQueue.IMPORT_EDUCATION_LEVELS));
        }

        public async Task ImportFaculties()
        {
            await _bus.PubSub.PublishAsync(new Empty(), x => x.WithTopic(RabbitMQQueue.IMPORT_FACULTIES));
        }

        public async Task ImportPrograms()
        {
            await _bus.PubSub.PublishAsync(new Empty(), x => x.WithTopic(RabbitMQQueue.IMPORT_PROGRAMS));
        }
    }
}
