using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;
using TravelAndAccommodationBookingPlatform.Core.Models;
using Microsoft.Extensions.Options;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_smtpSettings.UserName));
        email.To.Add(MailboxAddress.Parse(emailRequest.ToUserEmail));
        email.Subject = emailRequest.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = emailRequest.MessageBody
        };

        if (emailRequest.FileAttachments != null)
        {
            foreach (var attachment in emailRequest.FileAttachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }
        }

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(
            _smtpSettings.UserName,
            _smtpSettings.Password
        );
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
