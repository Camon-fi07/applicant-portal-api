using Dictionary.BL.Dto;
using Dictionary.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

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

        public static EducationDocumentTypeDto TranslateDocumentEntityToDto(EducationDocumentTypeEntity educationDocumentType)
        {
            return new EducationDocumentTypeDto()
            {
                Name = educationDocumentType.Name,
                CreateTime = educationDocumentType.CreateTime.ToUniversalTime(),
                EducationLevel = EducationLevelMapper.TranslateLevelEntityToDto(educationDocumentType.EducationLevel),
                Id = educationDocumentType.Id,
                NextEducationLevels = educationDocumentType.NextEducationLevels.Select(l => EducationLevelMapper.TranslateLevelEntityToDto(l)).ToList()
            };
        }
    }
}
