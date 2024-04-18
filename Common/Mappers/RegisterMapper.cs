using Common.Dto;
using Common.Enums;
using Common.Models;

namespace Common.Mappers
{
    public static class RegisterMapper
    {
        public static UserRegisterWithRoleModel TranslateApplicantRegisterToUserRegister(RegisterApplicantDto registerApplicantDto)
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
                Role =  RoleType.Applicant 
            };
        }

        public static UserRegisterWithRoleModel TranslateManagerRegisterToUserRegister(RegisterManagerDto registerManagerDto)
        {
            return new UserRegisterWithRoleModel()
            {
                BirthDate = registerManagerDto.BirthDate,
                Email = registerManagerDto.Email,
                Firstname = registerManagerDto.Firstname,
                Gender = registerManagerDto.Gender,
                Lastname = registerManagerDto.Lastname,
                Middlename = registerManagerDto.Middlename,
                Password = registerManagerDto.Password,
                PhoneNumber = registerManagerDto.PhoneNumber,
                Role = RoleType.Manager
            };
        }
    }
}
