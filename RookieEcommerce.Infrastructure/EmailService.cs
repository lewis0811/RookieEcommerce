using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RookieEcommerce.Infrastructure
{
    public class EmailService : RookieEcommerce.Application.IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

                if (smtpSettings == null)
                {
                    throw new InvalidOperationException("SmtpSettings section not found in configuration.");
                }

                using (var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
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
            catch (Exception ex)
            {

                throw;
            }
        }
    }

    public class SmtpSettings
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}