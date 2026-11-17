using Workly.EmailWorker.Abstractions;
using Workly.EmailWorker.Email;

namespace Workly.EmailWorker.Di;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddConfiguration(IConfiguration configuration)
        {
            services.Configure<EmailOptions>(
                configuration.GetSection(nameof(EmailOptions)));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        }
    }
}