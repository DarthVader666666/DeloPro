namespace Delopro.Server.Models
{
    public class SearchResultResponse
    {
        public int? ChapterId { get; set; }
        public int? ThemeId { get; set; }
        public string? ThemeTitle { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? SearchFragment { get; set; }
        public int Index { get; set; } = 0;
        public string? Text { get; set; }
    }
}
