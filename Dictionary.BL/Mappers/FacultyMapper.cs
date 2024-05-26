using Common.Dto.Dictionary;
using Common.Dto.RabbitMQ;
using Dictionary.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.BL.Mappers
{
    public static class FacultyMapper
    {
        public static AddFacultyDto TranslateFacultyDtoToAddDto(EducationFacultyDto faculty)
        {
            return new AddFacultyDto()
            {
                Id = faculty.Id,
                Name = faculty.Name,
            };
        }

        public static FacultyEntity TranslateFacultyDtoToEntity(EducationFacultyDto faculty)
        {
            return new FacultyEntity()
            {
                Id = faculty.Id,
                Name = faculty.Name,
                CreateTime = faculty.CreateTime.ToUniversalTime()
            };
        }

        public static AddFacultyDto TranslateFacultyEntityToAddDto(FacultyEntity faculty)
        {
            return new AddFacultyDto()
            {
                Id = faculty.Id,
                Name = faculty.Name,
            };
        }
    }
}
