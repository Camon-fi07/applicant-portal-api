using Common.Dto.Dictionary;
using Common.Dto.RabbitMQ;
using Dictionary.BL.Services;
using Dictionary.DAL.Entities;

namespace Dictionary.BL.Mappers
{
    public static class EducationDocumentTypeMapper
    {
        public static EducationDocumentTypeEntity TranslateDocumnetDtoToEntity(EducationDocumentTypeDto educationDocumentType, EducationLevelEntity educationLevel, List<EducationLevelEntity> nextEducationLevels)
        {
            return new EducationDocumentTypeEntity()
            {
                CreateTime = educationDocumentType.CreateTime.ToUniversalTime(),
                EducationLevel = educationLevel,
                Id = educationDocumentType.Id,
                Name = educationDocumentType.Name,
                NextEducationLevels = nextEducationLevels
            };
        }

        public static AddEducationDocumentTypeDto TranslateDocumnetDtoToAddDto(EducationDocumentTypeDto educationDocumentType)
        {
            var educationLevel = new AddEducationLevelDto()
            {
                Id = educationDocumentType.EducationLevel.Id,
                Name = educationDocumentType.EducationLevel.Name,
            };

            var nextEducationsLevels = educationDocumentType.NextEducationLevels?.Select(e => new AddEducationLevelDto()
            {
                Id = e.Id,
                Name = e.Name,
            }).ToList();

            return new AddEducationDocumentTypeDto()
            {
                EducationLevel = educationLevel,
                Id = educationDocumentType.Id,
                Name = educationDocumentType.Name,
                NextEducationLevels = nextEducationsLevels
            };
        }

        public static EducationDocumentTypeDto TranslateDocumentEntityToDto(EducationDocumentTypeEntity educationDocumentType)
        {
            return new EducationDocumentTypeDto()
            {
                Name = educationDocumentType.Name,
                CreateTime = educationDocumentType.CreateTime.ToUniversalTime(),
                EducationLevel = EducationLevelMapper.TranslateLevelEntityToDto(educationDocumentType.EducationLevel),
                Id = educationDocumentType.Id,
                NextEducationLevels = educationDocumentType.NextEducationLevels.Select(EducationLevelMapper.TranslateLevelEntityToDto).ToList()
            };
        }


        public static AddEducationDocumentTypeDto TranslateDocumentEntityToAddDto(EducationDocumentTypeEntity educationDocumentType)
        {
            var educationLevel = new AddEducationLevelDto()
            {
                Id = educationDocumentType.EducationLevel.Id,
                Name = educationDocumentType.EducationLevel.Name,
            };

            var nextEducationsLevels = educationDocumentType.NextEducationLevels?.Select(e => new AddEducationLevelDto()
            {
                Id = e.Id,
                Name = e.Name,
            }).ToList();

            return new AddEducationDocumentTypeDto()
            {
                EducationLevel = educationLevel,
                Id = educationDocumentType.Id,
                Name = educationDocumentType.Name,
                NextEducationLevels = nextEducationsLevels
            };
        }
    }
}
