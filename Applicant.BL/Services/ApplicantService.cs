using Applicant.BL.Dto;
using Applicant.BL.Services.IServices;
using Applicant.DAL;
using Applicant.DAL.Entities;
using Common.Dto;
using Common.Dto.RabbitMQ;
using Common.Enums;
using Common.Models;
using EasyNetQ;

namespace Applicant.BL.Services
{
    public class ApplicantService : IApplicantService {

        private readonly ApplicantDBContext _applicantDBContext;
        private readonly IBus _bus;

        public ApplicantService(ApplicantDBContext applicantDBContext, IBus bus)
        {
            _applicantDBContext = applicantDBContext;
            _bus = bus;
        }

        public async Task RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {
            var applicant = new ApplicantEntity()
            {
                Citizenship = registerApplicantDto.Citizenship,
                Education = registerApplicantDto.Education,
                Id = Guid.NewGuid(),
                UserId = registerApplicantDto.UserId,
                Passport = registerApplicantDto.Passport,
            };

            await _applicantDBContext.Applicants.AddAsync(applicant);

            await _bus.PubSub.PublishAsync(new AddRoleDto()
            {
                Role = RoleType.Applicant,
                UserId = registerApplicantDto.UserId
            });
        }

        Task IApplicantService.EditEducationDocument(EditEducationDocumentDto editEducationDocumentDto)
        {
            throw new NotImplementedException();
        }

        Task IApplicantService.EditPassport(EditPassportDto editPassportDto)
        {
            throw new NotImplementedException();
        }

        Task<TokenResponseDto> IApplicantService.EditProfile(EditApplicantProfileDto editApplicantProfileDto, Guid userId)
        {
            throw new NotImplementedException();
        }

        Task<EducationDocumentDto> IApplicantService.GetEducationDocument()
        {
            throw new NotImplementedException();
        }

        Task<PassportDto> IApplicantService.GetPasport()
        {
            throw new NotImplementedException();
        }

        Task<ApplicantProfileDto> IApplicantService.GetProfile(Guid userId)
        {
            throw new NotImplementedException();
        }

        Task IApplicantService.RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {
            throw new NotImplementedException();
        }
    }
}
