using Modules.Companies.Domain.Enums;

namespace Modules.Companies.Domain.Models;

public sealed class CompanyInvitation
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public string Email { get; set; } = null!;
    public CompanyRole Role { get; set; }
    public string TokenHash { get; set; } = null!;
    public InvitationStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid? AcceptedByUserId { get; set; }
    public DateTime? AcceptedAt { get; set; }
}
