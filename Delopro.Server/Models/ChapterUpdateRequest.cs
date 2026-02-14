namespace Delopro.Server.Models
{
    public class ChapterUpdateRequest
    {
        public int? ChapterId { get; set; }
        public int? UserId { get; set; }
        public string? ChapterTitle { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public ThemeUpdateRequest[]? Themes { get; set; }
    }
}
