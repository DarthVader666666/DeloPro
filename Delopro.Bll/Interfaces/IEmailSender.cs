namespace Delopro.Bll.Interfaces
{
    public interface IEmailSender
    {
        Task<(string? Message, bool Result)> SendEmailAsync(string? to, string? subject, string? body);
    }
}
