using Workly.EmailWorker.Email;

namespace Workly.EmailWorker.Abstractions;

public interface IEmailTemplateService
{
    EmailResult GetRegistrationConfirmation(string toEmail, string username, string code);
    EmailResult GetResetConfirmation(string toEmail, string code);
}