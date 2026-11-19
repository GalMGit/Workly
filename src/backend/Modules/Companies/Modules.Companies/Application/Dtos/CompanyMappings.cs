using Modules.Companies.Domain.Models;

namespace Modules.Companies.Application.Dtos;

public static class CompanyMappings
{
    public static CompanyDetails ToDetails(this Company company)
        => new(
            company.Id,
            company.Name,
            company.Slug,
            company.Description,
            company.LogoPath,
            company.WebsiteUrl,
            company.Industry,
            company.Size,
            company.Country,
            company.City,
            company.Address,
            company.ContactEmail,
            company.ContactPhone,
            company.OwnerUserId,
            company.Status,
            company.StatusReason,
            company.VerifiedAt,
            company.CreatedAt,
            company.UpdatedAt);
}
