using Microsoft.EntityFrameworkCore;
using Modules.Companies.Application.Dtos;
using Modules.Companies.Contracts.Events;
using Modules.Companies.Domain.Enums;
using Modules.Companies.Domain.Models;
using Modules.Companies.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.Attributes;
using Workly.Shared.ResultTools;

namespace Modules.Companies.Features.CreateCompany;

[Transactional(typeof(CompaniesDbContext))]
public sealed class CreateCompanyHandler(
    CompaniesDbContext context,
    IMessageBus bus)
{
    public async Task<Result<CompanyDetails>> Handle(
        CreateCompanyCommand command,
        CancellationToken ct)
    {
        var request = command.Request;
        var name = request.Name.Trim();
        var now = DateTime.UtcNow;

        var company = new Company
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Slug = await MakeUniqueSlugAsync(name, ct),
            Description = request.Description?.Trim(),
            WebsiteUrl = request.WebsiteUrl?.Trim(),
            Industry = request.Industry?.Trim(),
            Size = request.Size ?? CompanySize.Small,
            Country = request.Country?.Trim(),
            City = request.City?.Trim(),
            Address = request.Address?.Trim(),
            ContactEmail = request.ContactEmail?.Trim(),
            ContactPhone = request.ContactPhone?.Trim(),
            OwnerUserId = command.UserId,
            Status = CompanyStatus.Active,
            CreatedAt = now
        };

        company.Members.Add(new CompanyMember
        {
            CompanyId = company.Id,
            UserId = command.UserId,
            Role = CompanyRole.Owner,
            JoinedAt = now
        });

        await context.Companies.AddAsync(
            company, ct);

        await bus.PublishAsync(
            new CompanyCreated(
                company.Id,
                company.OwnerUserId,
                company.Name,
                company.Slug));

        return Result<CompanyDetails>.Success(
            company.ToDetails());
    }

    private async Task<string> MakeUniqueSlugAsync(
        string name,
        CancellationToken ct)
    {
        var baseSlug = BuildSlug(name);
        var slug = baseSlug;
        var suffix = 2;

        while (await context.Companies
                   .AnyAsync(x => x.Slug == slug, ct))
            slug = $"{baseSlug}-{suffix++}";

        return slug;
    }

    private static string BuildSlug(string name)
    {
        var chars = name
            .Trim()
            .ToLowerInvariant()
            .Select(ch =>
                char.IsLetterOrDigit(ch)
                    ? ch
                    : '-')
            .ToArray();

        var slug = new string(chars);

        while (slug.Contains("--", StringComparison.Ordinal))
            slug = slug.Replace("--", "-", StringComparison.Ordinal);

        slug = slug.Trim('-');

        if (slug.Length > 90)
            slug = slug[..90].Trim('-');

        return string.IsNullOrWhiteSpace(slug)
            ? "company"
            : slug;
    }
}
