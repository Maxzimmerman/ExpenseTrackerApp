using ExpenseTrackerApp.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;

namespace ExpenseTrackerApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient
            {
                Host = _configuration["Email:Smtp:Host"],
                Port = int.Parse(_configuration["Email:Smtp:Port"]),
                Credentials = new NetworkCredential(
                _configuration["Email:Smtp:Username"],
                _configuration["Email:Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Smtp:From"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
