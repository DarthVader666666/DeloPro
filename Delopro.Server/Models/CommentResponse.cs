namespace Delopro.Server.Models
{
    public class CommentResponse
    {
        public int CommentId { get; set; }
        public int ThemeId { get; set;  }
        public string? Text { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
        public int? UserId { get; set; }
        public string? Nickname { get; set; }
        public string? AvatarPath { get; set; }
    }
}
