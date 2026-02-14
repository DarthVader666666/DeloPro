namespace Delopro.Server.Models
{
    public class UserShortResponse
    {
        public int UserId { get; set; }
        public string? AvatarPath { get; set; }
        public string? Nickname { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string? Roles { get; set; }
        public int? Status { get; set; }
    }
}
