using Craft.Application.Common.Interface;
using System.Net.Mail;
using System.Net;

namespace Craft.Infrastucture.Services;
public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _from;

    public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string from)
    {
        _smtpClient = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };
        _from = from;
    }
    //public EmailService() ////This would be effective if i install MailHog and run it in Command Line
    //{
    //    _smtpClient = new SmtpClient("localhost", 1025);
    //    _from = "noreply@example.com";
    //}

    public async Task SendEmail(string to, string subject, string body)
    {
        var message = new MailMessage(_from, to, subject, body)
        {
            IsBodyHtml = true
        };

        await _smtpClient.SendMailAsync(message);

        await Task.FromResult(true);
    }
}
