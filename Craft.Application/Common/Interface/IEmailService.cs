namespace Craft.Application.Common.Interface;

public interface IEmailService
{
    Task SendEmail(string to, string subject, string body);
}
