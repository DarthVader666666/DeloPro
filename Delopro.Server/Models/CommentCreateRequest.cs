namespace Delopro.Server.Models
{
    public class CommentCreateRequest
    {
        public int ThemeId { get; set; }
        public string? Text { get; set; }
    }
}
