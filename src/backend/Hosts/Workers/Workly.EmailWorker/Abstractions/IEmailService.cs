namespace Workly.EmailWorker.Abstractions;

public interface IEmailService
{
    Task<bool> SendMailAsync(string toEmail, string subject, string body);
}