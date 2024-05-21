using Common.Dto.Applicant;

namespace Common.Dto.Application
{
    public class CreateApplicationDto
    {
        public required CreatePassportDto Passport { get; set; }
        public required CreateEducationDocumentDto Education { get; set; }
        public required List<CreateProgramPriorityDto> Programs { get; set; }
        public required string Citizenship { get; set; }
    }
}
