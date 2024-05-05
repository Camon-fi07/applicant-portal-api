using Admission.BL.Dto;
using Admission.BL.Services.IServices;
using Admission.DAL;
using Admission.DAL.Entities;
using Common.Dto.Applicant;
using Common.Enums;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AdmissionDBContext _admissionDBContext;
        private readonly IBus _bus;

        public ApplicationService(AdmissionDBContext admissionDBContext, IBus bus) {
            _admissionDBContext = admissionDBContext;
            _bus = bus;
        }

        public async Task CreateApplication(CreateApplicationDto createApplication, Guid userId)
        {
            ApplicationEntity applicationEntity = new ApplicationEntity() {
                Id = Guid.NewGuid(),
                Programs = createApplication.Programs.Select(p => new ProgramPriorityEntity()
                {
                    FacultyId = p.FacultyId,
                    Id = Guid.NewGuid(),
                    Priority = p.Priority,
                    ProgramId = p.ProgramId
                }).ToList(),
                Status = ApplicationStatus.Created,
                UserId = userId
            };

            await _bus.PubSub.PublishAsync(new RegisterApplicantDto()
            {
                ApplicationId = applicationEntity.Id,
                Citizenship = createApplication.Citizenship,
                Education = createApplication.Education,
                Passport = createApplication.Passport,
                UserId = userId
            });

            await _admissionDBContext.Applications.AddAsync(applicationEntity);

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task EditProgramPriorities(List<ProgramPriorityDto> programPriorities, Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);
            if(application == null) { throw new Exception(); }

            _admissionDBContext.ProgramPriorities.RemoveRange(application.Programs);

            await _admissionDBContext.SaveChangesAsync();

            var programsEntities = programPriorities.Select(p => new ProgramPriorityEntity()
            {
                FacultyId = p.FacultyId,
                Id = Guid.NewGuid(),
                Priority = p.Priority,
                ProgramId = p.ProgramId
            }).ToList();

            _admissionDBContext.ProgramPriorities.AddRange(programsEntities);

            await _admissionDBContext.SaveChangesAsync();

            application.Programs = programsEntities;

            await _admissionDBContext.SaveChangesAsync();
        }

        public async Task<ApplicationDto> GetApplication(Guid applicationId)
        {
            var application = await _admissionDBContext.Applications.Include(a => a.Programs).FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null) { throw new Exception(); }

            var passport = await _bus.Rpc.RequestAsync<Guid, PassportDto>(application.UserId);
            var education = await _bus.Rpc.RequestAsync<Guid, EducationDocumentDto>(application.UserId);
            var applicant = await _bus.Rpc.RequestAsync<Guid, ApplicantProfileDto>(application.UserId);

            return new ApplicationDto()
            {
                ApplicationId = applicationId,
                Status = application.Status,
                ProgramPriorities = application.Programs.Select(p => new ProgramPriorityDto() { FacultyId = p.FacultyId, Priority = p.Priority, ProgramId = p.ProgramId }).ToList(),
                Education = education,
                Passport = passport,
                User = applicant
            };
        }
    }
}
