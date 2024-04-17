using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace DTS.Common.Utility;

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }
    public async Task  SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient("in-v3.mailjet.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("fedd6d9678a0303cf29abd00008d6fa7", "a6402412b06b405bdaaea09b89f89f6f"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("baldoswill@gmail.com", "DTS System"),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            //TODO: Logger
        }
    }
}