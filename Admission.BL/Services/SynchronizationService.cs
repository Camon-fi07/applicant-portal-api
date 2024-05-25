using Admission.BL.Services.IServices;
using Admission.DAL.Entities;
using Admission.DAL;
using Common.Dto.RabbitMQ;
using Common.Enums;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Common.Dto;
using Common.Constants;
using Common.Dto.User;

namespace Admission.BL.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly AdmissionDBContext _admissionDBContext;
        private readonly IBus _bus;

        public SynchronizationService(AdmissionDBContext admissionDBContext, IBus bus)
        {
            _admissionDBContext = admissionDBContext;
            _bus = bus;
        }

        public async Task SychronizeGuides(GuideType guideType)
        {
            switch(guideType)
            {
                case GuideType.Faculties:
                    await SychronizeFaculties();
                    break;
                case GuideType.DocumentTypes:
                    await SychronizeEducationDocumentTypes();
                    break;
                case GuideType.EducationPrograms:
                    await SychronizePrograms();
                    break;
                case GuideType.EducationLevels:
                    await SychronizeEducationLevels();
                    break;
            }
        }

        public async Task SychronizeFaculties()
        {
            var originFaculties = _admissionDBContext.Faculties.Include(f => f.Programs).ToList();

            if (originFaculties.Count == 0) return;

            var facultiesIds = originFaculties.Select(f => f.Id).ToList();
            var newFaculties = await _bus.Rpc.RequestAsync<List<Guid>, List<AddFacultyDto>>(facultiesIds, x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_FACULTIES));

            await Parallel.ForEachAsync(originFaculties, async (originFaculty, _) =>
            {
                var newFaculty = newFaculties.FirstOrDefault(f => f.Id == originFaculty.Id);
                if (newFaculty == null)
                {
                    if (originFaculty.Programs != null)
                    {

                        await Parallel.ForEachAsync(originFaculty.Programs, async (p, _) =>
                        {
                            await RejectApplicationsForProgram(p);
                        });

                        _admissionDBContext.Programs.RemoveRange(originFaculty.Programs);
                        _admissionDBContext.Faculties.Remove(originFaculty);
                    };
                }
                else
                {
                    originFaculty.Name = newFaculty.Name;
                }
            });

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task SychronizeEducationLevels()
        {
            var originEducationLevels = _admissionDBContext.EducationLevels.Include(l => l.Programs).Include(l => l.CurrentEducationLevelDocumentTypes).ToList();

            if (originEducationLevels.Count == 0) return;

            var levelsIds = originEducationLevels.Select(l => l.Id).ToList();
            var newEducationLevels = await _bus.Rpc.RequestAsync<List<int>, List<AddEducationLevelDto>>(levelsIds, x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_EDUCATIONS));

            await Parallel.ForEachAsync(originEducationLevels, async (originEducationLevel, _) =>
            {
                var newEducationLevel = newEducationLevels.FirstOrDefault(l => l.Id == originEducationLevel.Id);
                if (newEducationLevel == null)
                {
                    if (originEducationLevel.Programs != null)
                    {
                        await Parallel.ForEachAsync(originEducationLevel.Programs, async (p, _) =>
                        {
                            await RejectApplicationsForProgram(p);
                        });

                        _admissionDBContext.Programs.RemoveRange(originEducationLevel.Programs);
                    };

                    if (originEducationLevel.CurrentEducationLevelDocumentTypes != null)
                    {
                        await Parallel.ForEachAsync(originEducationLevel.CurrentEducationLevelDocumentTypes, async (d, _) =>
                        {
                            await RefreshApplicationForDocument(d, true);
                        });

                        _admissionDBContext.EducationDocumentTypes.RemoveRange(originEducationLevel.CurrentEducationLevelDocumentTypes);
                    }

                    _admissionDBContext.EducationLevels.Remove(originEducationLevel);
                }
                else
                {
                    originEducationLevel.Name = originEducationLevel.Name;
                }
            });

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task SychronizeEducationDocumentTypes()
        {
            var originEducationDocumentTypes = _admissionDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).Include(d => d.NextEducationLevels).ToList();

            if (originEducationDocumentTypes.Count == 0) return;

            var documentIds = originEducationDocumentTypes.Select(l => l.Id).ToList();
            var newEducationDocumentTypes = await _bus.Rpc.RequestAsync<List<Guid>, List<AddEducationDocumentTypeDto>>(documentIds, x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_EDUCATION_DOCUMENT_TYPES));

            await Parallel.ForEachAsync(originEducationDocumentTypes, async (originEducationDocumentType, _) =>
            {
                var newEducationDocumentType = newEducationDocumentTypes.FirstOrDefault(d => d.Id == originEducationDocumentType.Id);
                if (newEducationDocumentType == null)
                {
                    await RefreshApplicationForDocument(originEducationDocumentType, true);

                    _admissionDBContext.EducationDocumentTypes.Remove(originEducationDocumentType);
                }
                else
                {
                    bool isChanged = false;

                    var educationLevel = await _admissionDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == newEducationDocumentType.EducationLevel.Id);
                    if (educationLevel != null) educationLevel.Name = newEducationDocumentType.EducationLevel.Name;
                    else
                    {
                        educationLevel = new EducationLevelCacheEntity()
                        {
                            Name = newEducationDocumentType.EducationLevel.Name,
                            Id = newEducationDocumentType.EducationLevel.Id,
                        };
                    }

                    if (educationLevel.Id != originEducationDocumentType.EducationLevel.Id) isChanged = true;

                    List<EducationLevelCacheEntity>? nextEducationLevels = null;

                    if (newEducationDocumentType.NextEducationLevels != null)
                    {
                        nextEducationLevels = new List<EducationLevelCacheEntity>();
                        foreach (var el in newEducationDocumentType.NextEducationLevels)
                        {
                            var nextEducationLevel = await _admissionDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == el.Id);
                            if (nextEducationLevel != null) nextEducationLevel.Name = el.Name;
                            else
                            {
                                nextEducationLevels.Add(new EducationLevelCacheEntity()
                                {
                                    Name = el.Name,
                                    Id = el.Id
                                });
                            }

                            if (!isChanged && originEducationDocumentType.NextEducationLevels.All(l => l.Id != nextEducationLevel.Id)) isChanged = true;
                        }
                    }
                    else isChanged = true;

                    originEducationDocumentType.Name = newEducationDocumentType.Name;
                    originEducationDocumentType.EducationLevel = educationLevel;
                    originEducationDocumentType.NextEducationLevels = nextEducationLevels;

                    if (isChanged) await RefreshApplicationForDocument(originEducationDocumentType);
                }
            });

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task SychronizePrograms()
        {
            var originPrograms = _admissionDBContext.Programs.Include(p => p.EducationLevel).ToList();

            if(originPrograms.Count == 0) return;

            var documentIds = originPrograms.Select(p => p.Id).ToList();
            var newPrograms =  await _bus.Rpc.RequestAsync<List<Guid>, List<AddProgramDto>>(documentIds, x => x.WithQueueName(RabbitMQQueue.SYCHRONIZE_PROGRAMS));

            await Parallel.ForEachAsync(originPrograms, async (originProgram, _) =>
            {
                var newProgram = newPrograms.FirstOrDefault(p => p.Id == originProgram.Id);
                if (newProgram == null)
                {
                    await RejectApplicationsForProgram(originProgram);

                    _admissionDBContext.Programs.Remove(originProgram);
                }
                else
                {
                    bool isChanged = false;

                    var educationLevel = await _admissionDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == newProgram.EducationLevel.Id);
                    if (educationLevel != null) educationLevel.Name = newProgram.EducationLevel.Name;
                    else
                    {
                        educationLevel = new EducationLevelCacheEntity()
                        {
                            Name = newProgram.EducationLevel.Name,
                            Id = newProgram.EducationLevel.Id
                        };
                    }

                    var faculty = await _admissionDBContext.Faculties.FirstOrDefaultAsync(f => f.Id == newProgram.Faculty.Id);
                    if (faculty != null) faculty.Name = newProgram.Faculty.Name;
                    else
                    {
                        faculty = new FacultyCacheEntity()
                        {
                            Name = newProgram.Faculty.Name,
                            Id = newProgram.Faculty.Id,
                        };
                    }

                    originProgram.Name = newProgram.Name;
                    originProgram.EducationLevel = educationLevel;
                    originProgram.Faculty = faculty;
                    originProgram.Code = newProgram.Code;

                    if (isChanged) await RejectApplicationsForProgram(originProgram, false);
                }
            });

            await _admissionDBContext.SaveChangesAsync();
        }

        private async Task RejectApplicationsForProgram(ProgramCacheEntity program, bool isDelete = true)
        {
            var applications = _admissionDBContext.Applications.Include(a => a.Programs).Include(a => a.ApplicantEducationDocumentType).Include(a => a.User).Where(a => a.Status != ApplicationStatus.Rejected && a.Programs.Any(p => p.ProgramId == program.Id)).ToList();

            await Parallel.ForEachAsync(applications, async (a, _) =>
            {
                if (isDelete)
                {
                    if (a.Status != ApplicationStatus.Rejected && a.Programs.Count == 1)
                    {
                        a.Status = ApplicationStatus.Rejected;
                        a.EditTime = DateTime.UtcNow;
                    }

                    var programPriority = a.Programs.FirstOrDefault(p => p.ProgramId == program.Id);
                    if (programPriority != null) _admissionDBContext.ProgramPriorities.Remove(programPriority);

                    await _bus.PubSub.PublishAsync(new EmailMessageDto()
                    {
                        Body = $"Программа {program.Name} больше недоступна",
                        Email = a.User.Email,
                        Subject = "Поступления"
                    });
                }
                else
                {
                    if (!IsProgramsValid(a.Programs, a.ApplicantEducationDocumentType))
                    {
                        a.Status = ApplicationStatus.Rejected;

                        await _bus.PubSub.PublishAsync(new EmailMessageDto()
                        {
                            Body = $"Программа {program.Name} больше недоступна",
                            Email = a.User.Email,
                            Subject = "Поступления"
                        });
                    }
                }
                
            });

            await _admissionDBContext.SaveChangesAsync();
        }

        private async Task RefreshApplicationForDocument(EducationDocumentTypeCacheEntity educationDocumentType, bool isDelete = false)
        {
            var applications = _admissionDBContext.Applications.Include(a => a.User).Include(a => a.ApplicantEducationDocumentType).Include(a => a.Programs).
                Where(a => a.ApplicantEducationDocumentType != null && a.ApplicantEducationDocumentType.Id == educationDocumentType.Id).ToList();

            await Parallel.ForEachAsync(applications, async (a, _) =>
            {
                if (isDelete)
                {
                    a.ApplicantEducationDocumentType = null;
                    a.Status = ApplicationStatus.Rejected;
                    a.EditTime = DateTime.UtcNow;

                    await _bus.PubSub.PublishAsync(new EmailMessageDto()
                    {
                        Body = $"Ваш документ об образовании больше не действителен, пожалуйста обновите его",
                        Email = a.User.Email,
                        Subject = "Поступления"
                    });
                }
                else if (!IsProgramsValid(a.Programs, educationDocumentType))
                {
                    a.Status = ApplicationStatus.Rejected;
                    a.EditTime = DateTime.UtcNow;

                    await _bus.PubSub.PublishAsync(new EmailMessageDto()
                    {
                        Body = $"Ваш уровень образования не позволяет вам поступить на одну из программ, пожалуйста обновите заявку",
                        Email = a.User.Email,
                        Subject = "Поступления"
                    });
                };

            });

            await _admissionDBContext.SaveChangesAsync();
        }

        private bool IsProgramsValid(List<ProgramPriorityEntity> programPriorities, EducationDocumentTypeCacheEntity educationDocumentType)
        {
            foreach (var programPrioritet in programPriorities)
            {
                var isNextLevel = educationDocumentType.NextEducationLevels != null && educationDocumentType.NextEducationLevels.Any(l => l.Id == programPrioritet.Program.EducationLevel.Id);
                var isCurrentLevel = educationDocumentType.EducationLevel.Id == programPrioritet.Program.EducationLevel.Id;

                if (!isNextLevel && !isCurrentLevel)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task SychronizeUser(AddUserDto addUser)
        {
            var user = await _admissionDBContext.Users.FirstOrDefaultAsync(u => u.Id == addUser.Id);
            if (user == null) return;

            user.Firstname = addUser.Firstname;
            user.Lastname = addUser.Lastname;
            user.Middlename = addUser.Middlename;
            user.Email = addUser.Email;

            await _admissionDBContext.SaveChangesAsync();
        }
    }
}
