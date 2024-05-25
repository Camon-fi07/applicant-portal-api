using Applicant.BL.Services.IServices;
using Applicant.DAL;
using Applicant.DAL.Entities;
using Common.Constants;
using Common.Dto.RabbitMQ;
using Common.Dto.User;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applicant.BL.Services
{
    public class ExternalService : IExternalService
    {
        private readonly IBus _bus;
        private readonly ApplicantDBContext _applicantDBContext;
        public ExternalService(IBus bus, ApplicantDBContext applicantDBContext)
        {
            _bus = bus;
            _applicantDBContext = applicantDBContext;
        }

        public async Task<EducationDocumentTypeCacheEntity> GetEducationDocument(Guid documentId)
        {
            var documentType = await _applicantDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).FirstOrDefaultAsync(d => d.Id == documentId);

            if (documentType == null)
            {
                await ImportEducationDocument(documentId);
                documentType = await _applicantDBContext.EducationDocumentTypes.Include(d => d.EducationLevel).FirstOrDefaultAsync(u => u.Id == documentId);

                if (documentType == null) throw new Exception();
            }

            return documentType;
        }

        public async Task ImportEducationDocument(Guid documentId)
        {
            var externalDocumentType = await _bus.Rpc.RequestAsync<Guid, AddEducationDocumentTypeDto>(documentId, x => x.WithQueueName(RabbitMQQueue.ADD_EDUCATION_DOCUMENT_TYPE));

            if (externalDocumentType == null) throw new Exception();

            var educationLevel = await _applicantDBContext.EducationLevels.FirstOrDefaultAsync(l => l.Id == externalDocumentType.EducationLevel.Id);
            if (educationLevel != null) educationLevel.Name = externalDocumentType.EducationLevel.Name;
            else
            {
                educationLevel = new EducationLevelCacheEntity()
                {
                    Name = externalDocumentType.EducationLevel.Name,
                    Id = externalDocumentType.EducationLevel.Id
                };
            }

            var documentType = new EducationDocumentTypeCacheEntity()
            {
                Name = externalDocumentType.Name,
                Id = externalDocumentType.Id,
                EducationLevel = educationLevel,
            };

            _applicantDBContext.EducationDocumentTypes.Add(documentType);
            await _applicantDBContext.SaveChangesAsync();
        }
    }
}
