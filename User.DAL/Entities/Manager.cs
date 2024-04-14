namespace User.DAL.Entities
{
    public class Manager
    {
        public Guid Id { get; set; }

        public required UserEntity User { get; set; }
    }
}
