using Applicant.BL.Services.IServices;
using Applicant.DAL;
using Applicant.DAL.Entities;
using Common.Constants;
using Common.Dto;
using Common.Dto.Applicant;
using Common.Dto.RabbitMQ;
using Common.Enums;
using Common.Models;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace Applicant.BL.Services
{
    public class ApplicantService : IApplicantService {

        private readonly ApplicantDBContext _applicantDBContext;
        private readonly IExternalService _externalService;
        private readonly IBus _bus;

        public ApplicantService(ApplicantDBContext applicantDBContext, IBus bus, IExternalService externalService)
        {
            _applicantDBContext = applicantDBContext;
            _bus = bus;
            _externalService = externalService;
        }

        public async Task<EducationDocumentDto> EditEducationDocument(EditEducationDocumentDto editEducationDocumentDto, Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Education).ThenInclude(e => e.Files).FirstOrDefaultAsync(a => a.Id == userId);

            if (applicant == null) throw new CustomExceptionModel(ErrorMessages.APPLICANT_NOT_FOUND, 400); 

            if(editEducationDocumentDto.Name != null) applicant.Education.Name = editEducationDocumentDto.Name;

            if (editEducationDocumentDto.Files != null)
            {
                if (editEducationDocumentDto.Files.Count < 1) throw new CustomExceptionModel(ErrorMessages.SCANS_LIST, 400); 

                applicant.Education.Files = editEducationDocumentDto.Files.Select(f => new FileEntity() { Name = f.Name, Id = f.Id }).ToList();
            }

            await _applicantDBContext.SaveChangesAsync();

            return new EducationDocumentDto()
            {
                EducationDocumentTypeId = applicant.Education.EducationDocument.Id,
                EducationLevelId = applicant.Education.EducationDocument.EducationLevel.Id,
                EducationLevelName = applicant.Education.EducationDocument.EducationLevel.Name,
                EducationName = applicant.Education.EducationDocument.Name,
                Id = applicant.Education.Id,
                Name = applicant.Education.Name,
                Files = applicant.Education.Files.Select(f => new FileDto() { Name = f.Name, Id = f.Id }).ToList(),
            };
        }

        public async Task<PassportDto> EditPassport(EditPassportDto editPassportDto, Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Passport).ThenInclude(p => p.Files).FirstOrDefaultAsync(a => a.Id == userId);

            if (applicant == null) throw new CustomExceptionModel(ErrorMessages.APPLICANT_NOT_FOUND, 400);

            if (editPassportDto.Issued != null) applicant.Passport.Issued = editPassportDto.Issued;
            if(editPassportDto.Series != null) applicant.Passport.Series = (int)editPassportDto.Series;
            if(editPassportDto.Number != null) applicant.Passport.Number = (int)editPassportDto.Number;
            if(editPassportDto.BirthPlace != null) applicant.Passport.BirthPlace = editPassportDto.BirthPlace;

            if(editPassportDto.Files != null)
            {
                if(editPassportDto.Files.Count < 1) throw new CustomExceptionModel(ErrorMessages.SCANS_LIST, 400);

                applicant.Passport.Files = editPassportDto.Files.Select(f => new FileEntity() { Name = f.Name, Id = f.Id }).ToList();
            }

            await _applicantDBContext.SaveChangesAsync();

            return new PassportDto()
            {
                BirthPlace = applicant.Passport.BirthPlace,
                Id = applicant.Passport.Id,
                Issued = applicant.Passport.Issued,
                Number = applicant.Passport.Number,
                Series = applicant.Passport.Series,
                Files = applicant.Passport.Files.Select(f => new FileDto() { Name = f.Name, Id = f.Id }).ToList()
            };
        }

        public async Task<ApplicantDataDto> EditApplicantData(EditApplicantDataDto editApplicantDataDto, Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.FirstOrDefaultAsync(a => a.Id == userId);

            if (applicant == null) throw new CustomExceptionModel(ErrorMessages.APPLICANT_NOT_FOUND, 400);

            if (editApplicantDataDto.Citizenship != null) applicant.Citizenship = editApplicantDataDto.Citizenship;

            await _applicantDBContext.SaveChangesAsync();

            var passport = await EditPassport(editApplicantDataDto.Passport, userId);
            var educationDOcument = await EditEducationDocument(editApplicantDataDto.EducationDocument, userId);

            return new ApplicantDataDto()
            {
                Citizenship = applicant.Citizenship,
                EducationDocument = educationDOcument,
                Passport = passport
            };
        }

        public async Task<EducationDocumentDto> GetEducationDocument(Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.
                Include(a => a.Education).ThenInclude(e => e.EducationDocument).ThenInclude(e => e.EducationLevel).
                Include(a => a.Education).ThenInclude(e => e.Files).FirstOrDefaultAsync(a => a.Id == userId);

            if (applicant == null) throw new CustomExceptionModel(ErrorMessages.APPLICANT_NOT_FOUND, 400);

            return new EducationDocumentDto()
            {
                EducationDocumentTypeId = applicant.Education.EducationDocument.Id,
                EducationLevelId = applicant.Education.EducationDocument.EducationLevel.Id,
                EducationLevelName = applicant.Education.EducationDocument.EducationLevel.Name,
                EducationName = applicant.Education.EducationDocument.Name,
                Id = applicant.Education.Id,
                Name = applicant.Education.Name,
                Files = applicant.Education.Files.Select(f => new FileDto() { Name = f.Name, Id = f.Id }).ToList(),
            };
        }

        public async Task<PassportDto> GetPassport(Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Passport).ThenInclude(p => p.Files).FirstOrDefaultAsync(a => a.Id == userId);

            if (applicant == null) throw new CustomExceptionModel(ErrorMessages.APPLICANT_NOT_FOUND, 400);

            return new PassportDto()
            {
                BirthPlace = applicant.Passport.BirthPlace,
                Files = applicant.Passport.Files.Select(f => new FileDto() { Name = f.Name, Id = f.Id }).ToList(),
                Id = Guid.NewGuid(),
                Issued = applicant.Passport.Issued,
                Number = applicant.Passport.Number,
                Series = applicant.Passport.Series
            };
        }

        public async Task<ApplicantDataDto> GetApplicantData(Guid userId)
        {
            var applicant = await _applicantDBContext.Applicants.Include(a => a.Education).ThenInclude(e => e.EducationDocument).ThenInclude(e => e.EducationLevel).
                Include(a => a.Education).ThenInclude(e => e.Files).
                Include(a => a.Passport).ThenInclude(p => p.Files).
                FirstOrDefaultAsync(a => a.Id == userId);

            if(applicant == null) throw new CustomExceptionModel(ErrorMessages.APPLICANT_NOT_FOUND, 400);

            return new ApplicantDataDto()
            {
                Citizenship = applicant.Citizenship,
                Passport = new PassportDto()
                {
                    BirthPlace = applicant.Passport.BirthPlace,
                    Id = applicant.Passport.Id,
                    Issued = applicant.Passport.Issued,
                    Number = applicant.Passport.Number,
                    Series = applicant.Passport.Series,
                    Files = applicant.Passport.Files.Select(f => new FileDto() { Name = f.Name, Id = f.Id }).ToList()
                },
                EducationDocument = new EducationDocumentDto()
                {
                    EducationDocumentTypeId = applicant.Education.EducationDocument.Id,
                    EducationLevelId = applicant.Education.EducationDocument.EducationLevel.Id,
                    EducationLevelName = applicant.Education.EducationDocument.EducationLevel.Name,
                    EducationName = applicant.Education.EducationDocument.Name,
                    Id = applicant.Education.Id,
                    Name = applicant.Education.Name,
                    Files = applicant.Education.Files.Select(f => new FileDto() { Name = f.Name, Id = f.Id }).ToList(),
                }
            };
        }

        public async Task RegisterApplicant(RegisterApplicantDto registerApplicantDto)
        {
            var educationDocumentType = await _externalService.GetEducationDocument(registerApplicantDto.Education.EducationDocumentTypeId);

            var passport = new PassportEntity()
            {
                BirthPlace = registerApplicantDto.Passport.BirthPlace,
                Files = registerApplicantDto.Passport.Files.Select(f => new FileEntity() { Name = f.Name, Id = f.Id}).ToList(),
                Id = Guid.NewGuid(),
                Issued = registerApplicantDto.Passport.Issued,
                Number = registerApplicantDto.Passport.Number,
                Series = registerApplicantDto.Passport.Series,
            };

            var education = new EducationEntity()
            {
                Files = registerApplicantDto.Education.Files.Select(f => new FileEntity() { Name = f.Name, Id = f.Id }).ToList(),
                Id = Guid.NewGuid(),
                EducationDocument = educationDocumentType,
                Name = registerApplicantDto.Education.Name
            };

            var applicant = new ApplicantEntity()
            {
                Citizenship = registerApplicantDto.Citizenship,
                Education = education,
                Id = registerApplicantDto.UserId,
                Passport = passport,
                Email = registerApplicantDto.Email
            };            

            await _applicantDBContext.Applicants.AddAsync(applicant);

            await _applicantDBContext.SaveChangesAsync();

            await _bus.PubSub.PublishAsync(new ChangeRoleDto()
            {
                Role = RoleType.Applicant,
                UserId = registerApplicantDto.UserId
            }, RabbitMQQueue.ADD_ROLE);
        }
    }
}
