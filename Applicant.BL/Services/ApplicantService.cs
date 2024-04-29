using Applicant.BL.Dto;
using Applicant.BL.Services.IServices;
using Applicant.DAL.Entities;
using Common.Dto;
using Common.Enums;
using Common.Models;

namespace Applicant.BL.Services
{
    public class ApplicantService : IApplicantService {

        public Task DeleteEducationDocument()
        {
            throw new NotImplementedException();
        }

        public Task DeletePassport()
        {
            throw new NotImplementedException();
        }

        public Task EditEducationDocument()
        {
            throw new NotImplementedException();
        }

        public Task EditPassport()
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponseDto> EditProfile(EditApplicantProfileDto editApplicantProfileDto)
        {
            throw new NotImplementedException();
        }

        public Task GetEducationDocument()
        {
            throw new NotImplementedException();
        }

        public Task GetPasport()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicantProfileDto> GetProfile()
        {
            throw new NotImplementedException();
        }

        public async Task<TokenResponseDto> RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {
            var applicant = new ApplicantEntity()
            {
                Id = Guid.NewGuid(),
                Citizenship = registerApplicantDto.Citizenship,
            };

            return new TokenResponseDto()
            {
                AccessToken = "",
                RefreshToken = ""
            };
        }
        private UserRegisterWithRoleModel TranslateApplicantRegisterToUserRegister(RegisterApplicantDto registerApplicantDto)
        {
            return new UserRegisterWithRoleModel()
            {
                BirthDate = registerApplicantDto.BirthDate,
                Email = registerApplicantDto.Email,
                Firstname = registerApplicantDto.Firstname,
                Gender = registerApplicantDto.Gender,
                Lastname = registerApplicantDto.Lastname,
                Middlename = registerApplicantDto.Middlename,
                Password = registerApplicantDto.Password,
                PhoneNumber = registerApplicantDto.PhoneNumber,
                Role = RoleType.Applicant
            };
        }

    }
}
