namespace Delopro.Server.Models
{
    public class UserLogInRequest
    {
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool Remember {  get; set; }
    }
}
