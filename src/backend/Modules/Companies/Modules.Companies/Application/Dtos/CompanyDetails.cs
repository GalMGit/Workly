using Modules.Companies.Domain.Enums;

namespace Modules.Companies.Application.Dtos;

public sealed record CompanyDetails(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoPath,
    string? WebsiteUrl,
    string? Industry,
    CompanySize Size,
    string? Country,
    string? City,
    string? Address,
    string? ContactEmail,
    string? ContactPhone,
    Guid OwnerUserId,
    CompanyStatus Status,
    string? StatusReason,
    DateTime? VerifiedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
