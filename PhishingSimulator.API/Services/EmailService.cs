using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PhishingSimulator.API.Models;

namespace PhishingSimulator.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPhishingEmailAsync(PhishingAttempt attempt)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"] ?? throw new InvalidOperationException("Email 'From' address not configured")));
            email.To.Add(MailboxAddress.Parse(attempt.RecipientEmail));
            email.Subject = "Important: Security Update Required";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                    <html>
                        <body>
                            <h2>Security Update Required</h2>
                            <p>Dear User,</p>
                            <p>We have detected unusual activity on your account. Please verify your information by clicking the link below:</p>
                            <p><a href='{attempt.TrackingLink}'>Verify Account</a></p>
                            <p>If you did not request this verification, please ignore this email.</p>
                            <p>Best regards,<br>Security Team</p>
                        </body>
                    </html>"
            };

            email.Body = builder.ToMessageBody();

            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? throw new InvalidOperationException("SMTP server not configured");
            var portStr = _configuration["EmailSettings:Port"] ?? throw new InvalidOperationException("SMTP port not configured");
            var username = _configuration["EmailSettings:Username"] ?? throw new InvalidOperationException("SMTP username not configured");
            var password = _configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("SMTP password not configured");

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                smtpServer,
                int.Parse(portStr),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(username, password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
} 