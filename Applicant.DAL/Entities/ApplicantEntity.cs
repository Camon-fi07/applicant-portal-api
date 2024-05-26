namespace Applicant.DAL.Entities
{
    public class ApplicantEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Citizenship { get; set; }
        public required PassportEntity Passport { get; set; }
        public required EducationEntity Education { get; set; }
    }
}
