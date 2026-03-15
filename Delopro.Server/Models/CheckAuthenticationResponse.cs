namespace Delopro.Server.Models
{
    public class CheckAuthenticationResponse
    {
        public bool IsAuthenticated {  get; set; }
        public AccountResponse? CurrentUser { get; set; }
    }
}
