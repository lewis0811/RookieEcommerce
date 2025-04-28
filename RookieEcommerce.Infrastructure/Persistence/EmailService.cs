using Microsoft.Extensions.Configuration;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Net;
using System.Net.Mail;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>()
                ?? throw new InvalidOperationException("SmtpSettings section not found in configuration.");
            using SmtpClient client = new(smtpSettings.Host, smtpSettings.Port);
            client.Credentials = new NetworkCredential(smtpSettings.Username!, smtpSettings.Password!);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings.Username),
                To = { email },
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}