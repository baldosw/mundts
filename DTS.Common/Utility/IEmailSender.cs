namespace DTS.Common.Utility;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}