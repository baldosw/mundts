using System.Net;
using System.Net.Mail;
using Mandrill;
using Mandrill.Model;
using RestSharp;
using RestSharp.Authenticators;

namespace DTS.Common.Utility;

public class EmailMessage
{
    public void SendEmail()
    {
        var options = new RestClientOptions("smtp.mandrillapp.com")
        {
            Authenticator = new HttpBasicAuthenticator("baldoswill@gmail.com", "5dced0d27497b5f370493f297eb64c79-us22")
        };
        
        var client = new RestClient(options);

        var request = new RestRequest("{domain}/messages", Method.Post);
        request.AddUrlSegment("domain", "smtp.mandrillapp.com");
        request.AddParameter("from", "baldoswill@gmail.com");
        request.AddParameter("to", "wbaldos85@gmail.com");
        request.AddParameter("subject", "Hello");
        request.AddParameter("text", "Testing some Mailgun awesomeness!");

        var response = client.Execute(request);
        // You can now handle the response according to your requirements.
        // For example, you might check if the request was successful.
        if (response.IsSuccessful)
        {
            // Handle success
            Console.WriteLine("Request successful!");
        }
        else
        {
            // Handle failure
            Console.WriteLine("Request failed with status code: " + response.StatusCode);
        }
    }
    
   
        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var smtpClient = new SmtpClient("in-v3.mailjet.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("fedd6d9678a0303cf29abd00008d6fa7", "a6402412b06b405bdaaea09b89f89f6f"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("baldoswill@gmail.com", "William"),
                Subject = "this is a test subject",
                Body = "hello world",
                IsBodyHtml = false,
            };

            mailMessage.To.Add("wbaldos85@gmail.com");

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        
    }
}