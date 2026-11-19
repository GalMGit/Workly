using FluentValidation;

namespace Modules.Companies.Features.CreateCompany;

public sealed class CreateCompanyValidator
    : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название компании обязательно.")
            .Length(2, 200)
            .WithMessage("Название должно быть от 2 до 200 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .WithMessage("Описание не должно превышать 4000 символов.");

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(512)
            .Must(BeHttpUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl))
            .WithMessage("Сайт должен быть корректным http(s)-адресом.");

        RuleFor(x => x.Industry)
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .MaximumLength(100);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Address)
            .MaximumLength(300);

        RuleFor(x => x.ContactEmail)
            .MaximumLength(256)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("Некорректный контактный email.");

        RuleFor(x => x.ContactPhone)
            .MaximumLength(32);

        RuleFor(x => x.Size)
            .IsInEnum()
            .When(x => x.Size.HasValue);
    }

    private static bool BeHttpUrl(string? value)
        => Uri.TryCreate(value, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp ||
               uri.Scheme == Uri.UriSchemeHttps);
}
