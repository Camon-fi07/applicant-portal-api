namespace Common.Dto.Applicant
{
    public class RegisterApplicantDto
    {
        public Guid ApplicationId { get; set; }
        public string Citizenship { get; set; }
        public CreatePassportDto Passport { get; set; }
        public Guid UserId { get; set; }
        public CreateEducationDocumentDto Education { get; set; }
    }
}
