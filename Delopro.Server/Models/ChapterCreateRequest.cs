namespace Delopro.Server.Models
{
    public class ChapterCreateRequest
    {
        public string? ChapterTitle { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
