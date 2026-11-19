using Modules.Companies.Domain.Enums;

namespace Modules.Companies.Domain.Models;

public sealed class CompanyMember
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public Guid UserId { get; set; }
    public CompanyRole Role { get; set; }
    public Guid? InvitedByUserId { get; set; }
    public DateTime JoinedAt { get; set; }
}
