namespace Delopro.Data.Entities
{
    public class Visit
    {
        public int VisitId { get; set; }
        public int? VisitorId { get; set; }
        public string? Url { get; set; }
        public DateTime VisitDate { get; set; }
        public Visitor? Visitor { get; set; }
    }
}
