namespace Applicant.DAL.Entities
{
    public class PassportEntity
    {
        public Guid Id { get; set; }
        public int Series { get; set; }
        public int Number { get; set; }
        public string BirthPlace { get; set; }
        public string Issued { get; set; }
        public List<FileEntity> Files { get; set; }
    }
}
