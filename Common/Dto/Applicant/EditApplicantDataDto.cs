using Common.Dto;

namespace Common.Dto.Applicant
{
    public class EditApplicantDataDto
    {
        public string? Citizenship { get; set; }
        public EditPassportDto Passport { get; set; }
        public EditEducationDocumentDto EducationDocument { get; set; }
    }
}
