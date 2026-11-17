using System.Reflection;
using Workly.EmailWorker.Abstractions;

namespace Workly.EmailWorker.Email;

public sealed class EmailTemplateService : IEmailTemplateService
{
    private const string TemplatesNamespace = "Email.Templates";

    public EmailResult GetRegistrationConfirmation(
        string toEmail,
        string username,
        string code)
    {
        var html = LoadTemplate("Register.html")
            .Replace("{{Username}}", username)
            .Replace("{{Code}}", code);

        return new EmailResult(
            toEmail,
            "Подтверждение регистрации в Workly",
            html);
    }

    public EmailResult GetResetConfirmation(
        string toEmail,
        string code)
    {
        var html = LoadTemplate("Reset.html")
            .Replace("{{Email}}", toEmail)
            .Replace("{{Code}}", code);

        return new EmailResult(
            toEmail,
            "Подтверждение смены пароля в Workly",
            html);
    }

    private static string LoadTemplate(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceName =
            $"{assembly.GetName().Name}.{TemplatesNamespace}.{fileName}";

        using var stream = assembly.GetManifestResourceStream(
            resourceName);

        if (stream is null)
        {
            var available = string.Join(
                ", ",
                assembly.GetManifestResourceNames());

            throw new FileNotFoundException(
                $"Embedded resource '{resourceName}' not found. " +
                $"Available resources: {available}",
                resourceName);
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
