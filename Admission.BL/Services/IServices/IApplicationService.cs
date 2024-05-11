using Admission.BL.Dto;
using Admission.BL.Enums;
using Admission.BL.Models;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admission.BL.Services.IServices
{
    public interface IApplicationService
    {
        public Task CreateApplication(CreateApplicationDto createApplication, Guid userId);
        public Task EditPrograms(List<ProgramPriorityDto> programPriorities, Guid applicationId);
        public Task<ApplicationDto> GetApplication(Guid applicationId);
        public Task<ApplicationsPagedListModel> GetApplications(int page, int size, string? name, Guid? programId, List<Guid>? faculties, ApplicationStatus? status, bool? haveManager, Guid? managerId, ApplicationsSortType? sortType);
        public Task ChangeApplicationStatus(Guid applicationId, ApplicationStatus status);
        public Task TakeApplication(Guid managerId, Guid applicationId);
        public Task Refuse(Guid managerId, Guid applicationId);
    }
}
