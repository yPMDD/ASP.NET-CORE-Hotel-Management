// Services/SmtpEmailSender.cs
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace HotelAurelia.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = null!;
        public string SenderPassword { get; set; } = null!;
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, "Hotel Aurelia"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(
                        _emailSettings.SenderEmail,
                        _emailSettings.SenderPassword),
                    EnableSsl = _emailSettings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = _emailSettings.UseDefaultCredentials
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("✅ Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send email to {Email}", email);
                // Don't throw - log and continue
            }
        }
    }
}