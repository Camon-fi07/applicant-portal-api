using Admission.BL.Dto;
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
        public Task EditProgramPriorities(List<ProgramPriorityDto> programPriorities, Guid applicationId);
        public Task<ApplicationDto> GetApplication(Guid applicationId);
    }
}
