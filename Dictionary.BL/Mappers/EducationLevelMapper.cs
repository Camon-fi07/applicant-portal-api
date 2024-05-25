using Common.Dto.Dictionary;
using Common.Dto.RabbitMQ;
using Dictionary.DAL.Entities;

namespace Dictionary.BL.Mappers
{
    public static class EducationLevelMapper
    {
        public static AddEducationLevelDto TranslateLevelDtoToAddDto(EducationLevelDto educationLevel)
        {
            return new AddEducationLevelDto()
            {
                Id = educationLevel.Id,
                Name = educationLevel.Name,
            };
        }

        public static EducationLevelEntity TranslateLevelDtoToEntity(EducationLevelDto educationLevelDto)
        {
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

        public static AddEducationLevelDto TranslateLevelEntityToAddDto(EducationLevelEntity educationLevelEntity)
        {
            return new AddEducationLevelDto()
            {
                Id = educationLevelEntity.Id,
                Name = educationLevelEntity.Name,
            };
        }
    }
}
