namespace User.DAL.Entities
{
    public class Applicant
    {
        public Guid Id { get; set; }

        public required UserEntity User { get; set; }

        public required string Citizenship { get; set; }
    }
}
