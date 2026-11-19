using Modules.Companies.Domain.Enums;

namespace Modules.Companies.Domain.Models;

public sealed class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? LogoPath { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Industry { get; set; }
    public CompanySize Size { get; set; } = CompanySize.Small;
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public Guid OwnerUserId { get; set; }
    public CompanyStatus Status { get; set; } = CompanyStatus.Active;
    public string? StatusReason { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<CompanyMember> Members { get; set; } = [];
    public ICollection<CompanyInvitation> Invitations { get; set; } = [];
}
