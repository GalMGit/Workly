using Modules.Companies.Domain.Enums;

namespace Modules.Companies.Features.CreateCompany;

public sealed record CreateCompanyRequest(
    string Name,
    string? Description,
    string? WebsiteUrl,
    string? Industry,
    CompanySize? Size,
    string? Country,
    string? City,
    string? Address,
    string? ContactEmail,
    string? ContactPhone);
