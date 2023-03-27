using Craft.Application.Common.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Serilog;

namespace Craft.Infrastucture.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _from;

    public EmailService(IConfiguration config)
    {
        string smtpServer = config.GetValue<string>("EmailSettings:SmtpServer");
        int smtpPort = config.GetValue<int>("EmailSettings:SmtpPort");
        string smtpUsername = config.GetValue<string>("EmailSettings:SmtpUsername");
        string smtpPassword = config.GetValue<string>("EmailSettings:SmtpPassword");
        _from = config.GetValue<string>("EmailSettings:From");

        _smtpClient = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        try
        {
            var message = new MailMessage(_from, to, subject, body)
            {
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "EmailService.SendEmail");
            throw;
        }
    }
}


//public EmailService() //This would be effective if i install MailHog and run it in Command Line
//{
//    _smtpClient = new SmtpClient("localhost", 1025);
//    _from = "noreply@example.com";
//}