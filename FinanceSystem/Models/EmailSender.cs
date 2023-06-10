using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FinanceSystem.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implement your email sending logic here
            _logger.LogInformation($"Sending email to {email} with subject: {subject}");
            _logger.LogInformation($"Email content: {htmlMessage}");

            // Replace this with your actual email sending code
            // For example, using an SMTP client or an email service provider API

            return Task.CompletedTask;
        }
    }
}
