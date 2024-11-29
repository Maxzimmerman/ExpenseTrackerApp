using ExpenseTrackerApp.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace ExpenseTrackerApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailSender(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public Task SendEmailAsync(string from, string subject, string message)
        {
            var smtpClient = new SmtpClient(_mailSettings.Server)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password)
            };

            return smtpClient.SendMailAsync(
                new MailMessage(
                    from: _mailSettings.UserName,
                    to: from,
                    subject: subject,
                    message
                ));
        }
    }
}
