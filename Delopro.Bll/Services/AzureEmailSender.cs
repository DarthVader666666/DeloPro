using Azure.Communication.Email;

using Delopro.Bll.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Delopro.Bll.Services
{
    public class AzureEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public AzureEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(string? Message, bool Result)> SendEmailAsync(string? to, string? subject, string? body)
        {
            var sender = _configuration["AzureEmailSender"];
            var connectionString = _configuration["AzureCommunicationService"];

            var client = new EmailClient(connectionString);

            EmailSendOperation? operation;

            try
            {
                operation = await client.SendAsync(
                    Azure.WaitUntil.Completed,
                    sender,
                    to,
                    subject,
                    body
                );
            }
            catch(Exception ex)
            {
                return (ex.Message, false);
            }

            return ($"{operation.Value.Status.ToString()}", operation?.Value.Status == EmailSendStatus.Succeeded);
        }
    }
}
