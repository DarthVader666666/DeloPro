namespace Delopro.Server.Models
{
    public class UserAccountUpdateRequestModel
    {
        public string? User { get; set; }
        public IFormFile? Avatar { get; set; } = null;
    }
}
