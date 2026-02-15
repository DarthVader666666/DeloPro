namespace Delopro.Server.Models
{
    public class AccountResponse
    {
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? RegisterDate { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? UserTitle { get; set; }
        public string? Info { get; set; }
        public string? AvatarPath { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string[]? Roles { get; set; } = [];
    }
}
