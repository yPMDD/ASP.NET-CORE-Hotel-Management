// Services/ConsoleEmailSender.cs
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HotelAurelia.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("=== EMAIL SENDING ===");
            _logger.LogInformation($"To: {email}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body:\n{htmlMessage}");
            _logger.LogInformation("=====================");

            // Also log credentials in a cleaner format
            if (subject.Contains("identifiants", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("=== RÉCEPTIONNISTE CREDENTIALS ===");
                _logger.LogInformation($"Email: {email}");
                // Extract password from HTML if possible, or log a message
                _logger.LogInformation("Password: [Check the email content above]");
                _logger.LogInformation("=================================");
            }

            return Task.CompletedTask;
        }
    }
}