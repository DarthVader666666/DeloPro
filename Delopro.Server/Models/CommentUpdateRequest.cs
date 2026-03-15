namespace Delopro.Server.Models
{
    public class CommentUpdateRequest
    {
        public int CommentId { get; set; }
        public int ThemeId { get; set; }
        public string? Text { get; set; }
    }
}
