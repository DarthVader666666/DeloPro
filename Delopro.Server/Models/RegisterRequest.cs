namespace Delopro.Server.Models
{
    public class RegisterRequest
    {
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? Password { get; set; }
        public DateTime? RegisterDate { get; set; }
    }
}
