using Applicant.BL.Services.IServices;
using Applicant.DAL;
using Applicant.DAL.Entities;
using Common.Constants;
using Common.Dto;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using Common.Enums;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace Applicant.BL.Services
{
    public class SynchronizationService:ISynchronizationService
    {
        private readonly ApplicantDBContext _applicantDBContext;
        private readonly IBus _bus;

        public SynchronizationService(ApplicantDBContext applicantDBContext, IBus bus)
        {
            _applicantDBContext = applicantDBContext;
            _bus = bus;
        }

        public async Task SychronizeGuides(GuideType guideType)
        {
            switch (guideType)
            {
                case GuideType.DocumentTypes:
                    await SychronizeEducationDocumentTypes();
                    break;
                case GuideType.EducationLevels:
                    await SychronizeEducationLevels();
                    break;
            }
        }

        public async Task SychronizeEducationLevels()
        {
            var originEducationLevels = _applicantDBContext.EducationLevels.Include(l => l.CurrentEducationLevelDocumentTypes).ToList();

            if (originEducationLevels.Count == 0) return;

            var levelsIds = originEducationLevels.Select(l => l.Id).ToList();
            var newEducationLevels = await _bus.Rpc.RequestAsync<List<int>, List<AddEducationLevelDto>>(levelsIds, x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_EDUCATIONS));

            await Parallel.ForEachAsync(originEducationLevels, async (originEducationLevel, _) =>
            {
                var newEducationLevel = newEducationLevels.FirstOrDefault(l => l.Id == originEducationLevel.Id);
                if (newEducationLevel == null)
                {
                    if (originEducationLevel.CurrentEducationLevelDocumentTypes != null)
                    {
                        await Parallel.ForEachAsync(originEducationLevel.CurrentEducationLevelDocumentTypes, async (d, _) =>
                        {
                            var applicants = _applicantDBContext.Applicants.Include(a => a.Education).ThenInclude(e => e.EducationDocument).Where(a => a.Education.EducationDocument.Id == d.Id);

                            await Parallel.ForEachAsync(applicants, async (a, _) =>
                            {
                                await _bus.PubSub.PublishAsync(new EmailMessageDto()
                                {
                                    Body = $"Ваш документ об образовании больше не действителен",
                                    Email = a.Email,
                                    Subject = "Поступления"
                                });

                                _applicantDBContext.Educations.Remove(a.Education);
                            });
                        });

                        _applicantDBContext.EducationDocumentTypes.RemoveRange(originEducationLevel.CurrentEducationLevelDocumentTypes);
                    }

                    _applicantDBContext.EducationLevels.Remove(originEducationLevel);
                }
                else
                {
                    originEducationLevel.Name = originEducationLevel.Name;
                }
            });

            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task SychronizeEducationDocumentTypes()
        {
            var originEducationDocumentTypes = _applicantDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).ToList();

            if (originEducationDocumentTypes.Count == 0) return;

            var documentIds = originEducationDocumentTypes.Select(l => l.Id).ToList();
            var newEducationDocumentTypes = await _bus.Rpc.RequestAsync<List<Guid>, List<AddEducationDocumentTypeDto>>(documentIds, x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_EDUCATION_DOCUMENT_TYPES));

            await Parallel.ForEachAsync(originEducationDocumentTypes, async (originEducationDocumentType, _) =>
            {
                var newEducationDocumentType = newEducationDocumentTypes.FirstOrDefault(d => d.Id == originEducationDocumentType.Id);
                if (newEducationDocumentType == null)
                {
                    var applicants = _applicantDBContext.Applicants.Include(a => a.Education).ThenInclude(e => e.EducationDocument).Where(a => a.Education.EducationDocument.Id == originEducationDocumentType.Id);

                    await Parallel.ForEachAsync(applicants, async (a, _) =>
                    {
                        await _bus.PubSub.PublishAsync(new EmailMessageDto()
                        {
                            Body = $"Ваш документ об образовании больше не действителен",
                            Email = a.Email,
                            Subject = "Поступления"
                        });

                        _applicantDBContext.Educations.Remove(a.Education);
                    });

                    _applicantDBContext.EducationDocumentTypes.Remove(originEducationDocumentType);
                }
                else
                {
                    var educationLevel = await _applicantDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == newEducationDocumentType.EducationLevel.Id);
                    if (educationLevel != null) educationLevel.Name = newEducationDocumentType.EducationLevel.Name;
                    else
                    {
                        educationLevel = new EducationLevelCacheEntity()
                        {
                            Name = newEducationDocumentType.EducationLevel.Name,
                            Id = newEducationDocumentType.EducationLevel.Id,
                        };
                    }

                    originEducationDocumentType.Name = newEducationDocumentType.Name;
                    originEducationDocumentType.EducationLevel = educationLevel;
                }
            });

            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task SychronizeProfile(AddUserDto addUserDto)
        {
            var applicant = await _applicantDBContext.Applicants.FirstOrDefaultAsync(a => a.Id == addUserDto.Id);

            if (applicant == null) return;
            
            applicant.Email = addUserDto.Email;

            await _applicantDBContext.SaveChangesAsync();
        }
    }
}
