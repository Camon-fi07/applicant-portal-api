using Common.Dto.Dictionary;
using Common.Dto.RabbitMQ;
using Dictionary.DAL.Entities;

namespace Dictionary.BL.Mappers
{
    public static class ProgramMapper
    {
        public static AddProgramDto TranslateProgramDtoToAddDto(EducationProgramDto educationProgram) {
            var addEducationLevel = new AddEducationLevelDto()
            {
                Id = educationProgram.EducationLevel.Id,
                Name = educationProgram.EducationLevel.Name,
            };

            var addFaculty = new AddFacultyDto()
            {
                Id = educationProgram.Faculty.Id,
                Name = educationProgram.Faculty.Name,
            };

            return new AddProgramDto()
            {
                Code = educationProgram.Code,
                EducationLevel = addEducationLevel,
                Name = educationProgram.Name,
                Id = educationProgram.Id,
                Faculty = addFaculty
            };
        }

        public static AddProgramDto TranslateProgramEntityToAddDto(EducationProgramEntity educationProgram)
        {
            var addEducationLevel = new AddEducationLevelDto()
            {
                Id = educationProgram.EducationLevel.Id,
                Name = educationProgram.EducationLevel.Name,
            };

            var addFaculty = new AddFacultyDto()
            {
                Id = educationProgram.Faculty.Id,
                Name = educationProgram.Faculty.Name,
            };

            return new AddProgramDto()
            {
                Code = educationProgram.Code,
                EducationLevel = addEducationLevel,
                Name = educationProgram.Name,
                Id = educationProgram.Id,
                Faculty = addFaculty
            };
        }
    }
}
