using Dictionary.BL.Dto;
using Dictionary.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.BL.Mappers
{
    public static class EducationLevelMapper
    {
        public static EducationLevelEntity TranslateLevelDtoToEntity(EducationLevelDto educationLevelDto) {
            return new EducationLevelEntity()
            {
                Id = educationLevelDto.Id,
                Name = educationLevelDto.Name,
            };
        }

        public static EducationLevelDto TranslateLevelEntityToDto(EducationLevelEntity educationLevelEntity)
        {
            return new EducationLevelDto()
            {
                Id = educationLevelEntity.Id,
                Name = educationLevelEntity.Name,
            };
        }
    }
}
