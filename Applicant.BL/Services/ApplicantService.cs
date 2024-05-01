using Applicant.BL.Dto;
using Applicant.BL.Services.IServices;
using Applicant.DAL;
using Applicant.DAL.Entities;
using Common.Dto;
using Common.Dto.RabbitMQ;
using Common.Enums;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

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

        public async Task<EducationDocumentDto> EditEducationDocument(EditEducationDocumentDto editEducationDocumentDto, Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Education).FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null) { throw new Exception(); }

            if (editEducationDocumentDto.EducationDocumentTypeId != null) applicant.Education.EducationDocumentTypeId = (Guid)editEducationDocumentDto.EducationDocumentTypeId;
            if(editEducationDocumentDto.Name != null) applicant.Education.Name = editEducationDocumentDto.Name;

            List<FileResponseDto>? educationFiles = null;

            if (editEducationDocumentDto.Files != null)
            {
                if (editEducationDocumentDto.Files.Count < 1) { throw new Exception(); }

                applicant.Education.Files = editEducationDocumentDto.Files;
            }

            await _applicantDBContext.SaveChangesAsync();

            return new EducationDocumentDto()
            {
               EducationDocumentTypeId = applicant.Education.EducationDocumentTypeId,
               Id = applicant.Education.Id,
               Name = applicant.Education.Name,
               Files = applicant.Education.Files
            };
        }

        public async Task<PassportDto> EditPassport(EditPassportDto editPassportDto, Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Passport).FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null) { throw new Exception(); }

            if(editPassportDto.Issued != null) applicant.Passport.Issued = editPassportDto.Issued;
            if(editPassportDto.Series != null) applicant.Passport.Series = (int)editPassportDto.Series;
            if(editPassportDto.Number != null) applicant.Passport.Number = (int)editPassportDto.Number;
            if(editPassportDto.BirthDate != null) applicant.Passport.BirthDate = (DateTime)editPassportDto.BirthDate;
            if(editPassportDto.BirthPlace != null) applicant.Passport.BirthPlace = editPassportDto.BirthPlace;

            List<FileResponseDto>? passportFiles = null;

            if(editPassportDto.Files != null)
            {
                if(editPassportDto.Files.Count < 1) { throw new Exception(); }

                applicant.Passport.Files = editPassportDto.Files;
            }

            await _applicantDBContext.SaveChangesAsync();

            return new PassportDto()
            {
                BirthDate = applicant.Passport.BirthDate,
                BirthPlace = applicant.Passport.BirthPlace,
                Id = applicant.Passport.Id,
                Issued = applicant.Passport.Issued,
                Number = applicant.Passport.Number,
                Series = applicant.Passport.Series,
                Files = applicant.Passport.Files
            };
        }

        public async Task<ApplicantProfileDto> EditProfile(EditApplicantProfileDto editApplicantProfileDto, Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null) { throw new Exception(); }

            if(editApplicantProfileDto.Citizenship != null) applicant.Citizenship = editApplicantProfileDto.Citizenship;

            await _applicantDBContext.SaveChangesAsync();

            var editProfileDto = new EditProfileDto()
            {
                BirthDate = editApplicantProfileDto.BirthDate,
                Email = editApplicantProfileDto.Email,
                Firstname = editApplicantProfileDto.Firstname,
                Gender = editApplicantProfileDto.Gender,
                Lastname = editApplicantProfileDto.Lastname,
                Middlename = editApplicantProfileDto.Middlename,
                PhoneNumber = editApplicantProfileDto.PhoneNumber
            };

            var profile = _bus.Rpc.Request<EditUserProfileDto, ProfileDto>(new EditUserProfileDto()
            {
                EditProfileDto = editProfileDto,
                UserId = userId,
            });

            return new ApplicantProfileDto()
            {
                BirthDate = profile.BirthDate,
                Citizenship = applicant.Citizenship,
                Email = profile.Email,
                FirstName = profile.FirstName,
                Gender = profile.Gender,
                LastName = profile.LastName,
                MiddleName = profile.MiddleName,
                PhoneNumber = profile.PhoneNumber,
                Roles = profile.Roles
            };
        }

        public async Task<EducationDocumentDto> GetEducationDocument(Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Education).FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null) { throw new Exception(); }

            return new EducationDocumentDto()
            {
                Files = applicant.Education.Files,
                Id = Guid.NewGuid(),
                EducationDocumentTypeId = applicant.Education.EducationDocumentTypeId,
                Name = applicant.Education.Name,
            };
        }

        public async Task<PassportDto> GetPassport(Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Passport).FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null) { throw new Exception(); }

            return new PassportDto()
            {
                BirthDate = applicant.Passport.BirthDate,
                BirthPlace = applicant.Passport.BirthPlace,
                Files = applicant.Passport.Files,
                Id = Guid.NewGuid(),
                Issued = applicant.Passport.Issued,
                Number = applicant.Passport.Number,
                Series = applicant.Passport.Series
            };
        }

        public async Task<ApplicantProfileDto> GetProfile(Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);

            if(applicant == null) { throw new Exception(); }

            var user = _bus.Rpc.Request<Guid, ProfileDto>(applicant.UserId);

            return new ApplicantProfileDto()
            {
                BirthDate = user.BirthDate,
                Citizenship = applicant.Citizenship,
                Email = user.Email,
                FirstName = user.FirstName,
                Gender = user.Gender,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                Roles = user.Roles,
            };
        }

        public async Task RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {

            var passport = new PassportEntity()
            {
                BirthDate = registerApplicantDto.Passport.BirthDate.ToUniversalTime(),
                BirthPlace = registerApplicantDto.Passport.BirthPlace,
                Files = registerApplicantDto.Passport.Files,
                Id = Guid.NewGuid(),
                Issued = registerApplicantDto.Passport.Issued,
                Number = registerApplicantDto.Passport.Number,
                Series = registerApplicantDto.Passport.Series,
            };

            var education = new EducationEntity()
            {
                Files = registerApplicantDto.Education.Files,
                Id = Guid.NewGuid(),
                EducationDocumentTypeId = registerApplicantDto.Education.EducationDocumentTypeId,
                Name = registerApplicantDto.Education.Name
            };

            var applicant = new ApplicantEntity()
            {
                Citizenship = registerApplicantDto.Citizenship,
                Education = education,
                Id = Guid.NewGuid(),
                UserId = registerApplicantDto.UserId,
                Passport = passport,
                Applications = new List<Guid>() { registerApplicantDto.ApplicationId }
            };

            await _applicantDBContext.Applicants.AddAsync(applicant);

            await _applicantDBContext.SaveChangesAsync();

            await _bus.PubSub.PublishAsync(new AddRoleDto()
            {
                Role = RoleType.Applicant,
                UserId = registerApplicantDto.UserId
            });
        }
    }
}
