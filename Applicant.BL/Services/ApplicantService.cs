using Applicant.BL.Dto;
using Applicant.BL.Services.IServices;
using Common.Dto;
using Common.RabbitMQ;


namespace Applicant.BL.Services
{
    public class ApplicantService : IApplicantService
    {
        RabbitMqService _rabbitMqService;

        public ApplicantService(RabbitMqService rabbitMqService) {
            _rabbitMqService = rabbitMqService;
        }

        public Task<TokenResponseDto> EditApplicantProfile(EditApplicantProfileDto editApplicantProfileDto)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicantProfileDto> GetApplicantProfile()
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {
            throw new NotImplementedException();
        }
    }
}
