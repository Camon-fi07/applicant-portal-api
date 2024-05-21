using Common.Enums;
using Microsoft.AspNetCore.Identity;
using User.BL.Services.IServices;
using User.DAL.Entities;
using User.DAL;
using Microsoft.AspNetCore.Http;
using User.BL.Dto;
using Common.Constants;
using EasyNetQ;
using Common.Dto.Applicant;
using Common.Dto.RabbitMQ;
using Common.Dto.User;

namespace User.BL.Services
{
    public class ProfileService:IProfileService
    {
        UserManager<UserEntity> _userManager;
        UserDBContext _userDBContext;
        IBus _bus;
        public ProfileService(UserManager<UserEntity> userManager, UserDBContext userDBContext, IBus bus) {
            _userManager = userManager;
            _userDBContext = userDBContext;
            _bus = bus;
        }

        public async Task<List<RoleType?>> GetUserRoles(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if(user == null) { throw new Exception(); }

            var roles = (await _userManager.GetRolesAsync(user)).Select(role => {
                var roleType = GetRoleType(role);
                if (roleType is null) throw new UnauthorizedAccessException();

                return roleType!;
            });

            return roles.ToList();
        }

        public async Task<AddUserDto> GetShortProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { throw new Exception(); }

            return new AddUserDto()
            {
                Email = user.Email,
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Middlename = user.Middlename,
            };
        }

        public async Task<ProfileDto> GetProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { throw new Exception(); }

            var roles = (await _userManager.GetRolesAsync(user)).Select(role => {
                var roleType = GetRoleType(role);
                if (roleType is null) throw new UnauthorizedAccessException();

                return roleType!;
            });

            return new ProfileDto()
            {
                BirthDate = user.BirthDate,
                Email = user.Email,
                FirstName = user.Firstname,
                Gender = user.Gender,
                LastName = user.Lastname,
                MiddleName = user.Middlename,
                PhoneNumber = user.PhoneNumber,
            };
        }

        private RoleType? GetRoleType(string role)
        {
            foreach (var roleType in Enum.GetValues(typeof(RoleType)).Cast<RoleType>())
            {
                if (roleType.ToString() == role) return roleType;
            }
            return null;
        }

        public async Task<ProfileDto> EditProfile(Guid userId, EditProfileDto editProfileDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { throw new Exception(); }

            if (editProfileDto.Lastname != null) user.Lastname = editProfileDto.Lastname;
            if (editProfileDto.PhoneNumber != null) user.PhoneNumber = editProfileDto.PhoneNumber;
            if (editProfileDto.Firstname != null) user.Firstname = editProfileDto.Firstname;
            if(editProfileDto.Middlename != null) user.Middlename = editProfileDto.Middlename;
            if(editProfileDto.Gender != null) user.Gender = (Gender)editProfileDto.Gender;
            if(editProfileDto.BirthDate != null) user.BirthDate = (DateOnly)editProfileDto.BirthDate;

            if(editProfileDto.Email != null) {
                user.Email = editProfileDto.Email;
                user.UserName = editProfileDto.Email;

                var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(user, editProfileDto.Email);
                var emailChangeResult = await _userManager.ChangeEmailAsync(user, editProfileDto.Email, emailChangeToken);

                if (!emailChangeResult.Succeeded)
                {
                    throw new BadHttpRequestException("Произошла ошибка");
                }
            }

            await _bus.PubSub.PublishAsync(new EditProfileDto()
            {
                Email = editProfileDto.Email,
                Firstname = editProfileDto.Firstname,
                Lastname = editProfileDto.Lastname,
                Middlename = editProfileDto.Middlename,
                BirthDate = editProfileDto.BirthDate,
                Gender = editProfileDto.Gender,
                PhoneNumber = editProfileDto.PhoneNumber
            }, RabbitMQQueue.SYNCHRONIZE_PROFILE);

            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();

            return await GetProfile(userId);
        }

        public async Task ResetPassword(Guid userId, ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new Exception();
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                throw new BadHttpRequestException("Произошла ошибка");
            }

            await _userManager.UpdateAsync(user);
            await _userDBContext.SaveChangesAsync();
        }

        public async Task AddRole(RoleType role, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new Exception();

            await _userManager.AddToRoleAsync(user, role.ToString());
        }

        public async Task DeleteRole(RoleType role, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new Exception();

            await _userManager.RemoveFromRoleAsync(user, role.ToString());
        }

        public async Task<ApplicantProfileDto> EditApplicantProfile(Guid userId, EditApplicantProfileDto editApplicantProfileDto)
        {
            var applicantData = _bus.Rpc.Request<EditApplicantDataRabbitDto, ApplicantDataDto>(new EditApplicantDataRabbitDto()
            {
                UserId = userId,
                EditApplicantDataDto = new EditApplicantDataDto()
                {
                    Citizenship = editApplicantProfileDto.Citizenship,
                    EducationDocument = editApplicantProfileDto.EducationDocument,
                    Passport = editApplicantProfileDto.Passport,

                }
            });

            var profile = await EditProfile(userId, editApplicantProfileDto);

            return new ApplicantProfileDto()
            {
                Email = profile.Email,
                Passport = applicantData.Passport,
                BirthDate = profile.BirthDate,
                Citizenship = applicantData.Citizenship,
                EducationDocument = applicantData.EducationDocument,
                FirstName = profile.FirstName,
                Gender = profile.Gender,
                LastName = profile.LastName,
                MiddleName = profile.MiddleName,
                PhoneNumber = profile.PhoneNumber,
            };
        }

        public async Task<ApplicantProfileDto> GetApplicantProfile(Guid userId)
        {
            var applicantData = await _bus.Rpc.RequestAsync<Guid, ApplicantDataDto>(userId);

            var profile = await GetProfile(userId);

            return new ApplicantProfileDto()
            {
                Email = profile.Email,
                Passport = applicantData.Passport,
                BirthDate = profile.BirthDate,
                Citizenship = applicantData.Citizenship,
                EducationDocument = applicantData.EducationDocument,
                FirstName = profile.FirstName,
                Gender = profile.Gender,
                LastName = profile.LastName,
                MiddleName = profile.MiddleName,
                PhoneNumber = profile.PhoneNumber,
            };
        }
    }
}
