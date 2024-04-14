using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.BL.Dto.Auth;
using User.BL.Models;

namespace User.BL.Mappers
{
    public static class RegisterMapper
    {
        public static UserRegisterModel TranslateApplicantRegisterToUserRegister(RegisterApplicantDto registerApplicantDto)
        {
            return new UserRegisterModel()
            {
                BirthDate = registerApplicantDto.BirthDate,
                Email = registerApplicantDto.Email,
                Firstname = registerApplicantDto.Firstname,
                Gender = registerApplicantDto.Gender,
                Lastname = registerApplicantDto.Lastname,
                Middlename = registerApplicantDto.Middlename,
                Password = registerApplicantDto.Password,
                PhoneNumber = registerApplicantDto.PhoneNumber,
            };
        }

        public static UserRegisterModel TranslateManagerRegisterToUserRegister(RegisterManagerDto registerManagerDto)
        {
            return new UserRegisterModel()
            {
                BirthDate = registerManagerDto.BirthDate,
                Email = registerManagerDto.Email,
                Firstname = registerManagerDto.Firstname,
                Gender = registerManagerDto.Gender,
                Lastname = registerManagerDto.Lastname,
                Middlename = registerManagerDto.Middlename,
                Password = registerManagerDto.Password,
                PhoneNumber = registerManagerDto.PhoneNumber,
            };
        }
    }
}
