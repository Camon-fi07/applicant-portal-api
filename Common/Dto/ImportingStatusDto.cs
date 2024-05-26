using Common.Enums;

namespace Common.Dto
{
    public class ImportingStatusDto
    {
        public Guid Id { get; set; }
        public int StatusCode { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public GuideType GuideType { get; set; }
    }
}
