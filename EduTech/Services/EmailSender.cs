using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EduTech.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailClient _emailClient;
        private readonly ILogger<EmailSender> _logger;
        private const string SenderAddress = "DoNotReply@93f2d740-e532-4a56-bcfd-4844eae76c3e.azurecomm.net";

        public EmailSender(EmailClient emailClient, ILogger<EmailSender> logger)
        {
            _emailClient = emailClient;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailMessage = new EmailMessage(
                    senderAddress: SenderAddress,
                    recipientAddress: email,
                    content: new EmailContent(subject)
                    {
                        Html = htmlMessage
                    }
                );

                EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                    WaitUntil.Started,
                    emailMessage
                );

                // Optional: Wait for the operation to complete
                await emailSendOperation.WaitForCompletionAsync();
                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {email}");
                throw;
            }
        }
    }
}
