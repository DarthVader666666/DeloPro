namespace Delopro.Data.Entities
{
    public class Visitor
    {
        public int VisitorId { get; set; }
        public int? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public User? User { get; set; }
        public virtual ICollection<Visit>? Visits { get; set; }
    }
}