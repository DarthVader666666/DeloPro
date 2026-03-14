using MailKit.Net.Smtp;
using MimeKit;

using Delopro.Bll.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Delopro.Bll.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly CryptoService _cryptoService;

        public SmtpEmailSender(IConfiguration configuration, CryptoService cryptoService)
        {
            _configuration = configuration;
            _cryptoService = cryptoService;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DeloPro", _configuration["SmtpEmailSender:UserName"]));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    _configuration["SmtpEmailSender:Host"],
                    587,
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["SmtpEmailSender:UserName"],
                    _cryptoService.Decrypt(_configuration["SmtpEmailSender:Password"])
                );

                var response = await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return response.StartsWith("250");
            }
            catch
            {
                return false;
            }
        }
    }
}
