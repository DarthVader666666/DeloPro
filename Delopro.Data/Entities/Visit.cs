namespace Delopro.Data.Entities
{
    public class Visit
    {
        public int VisitId { get; set; }
        public int? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? Url { get; set; }
        public DateTime VisitDate { get; set; }
        public User? User { get; set; }
    }
}
