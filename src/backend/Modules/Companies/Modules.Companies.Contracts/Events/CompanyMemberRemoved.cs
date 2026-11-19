namespace Modules.Companies.Contracts.Events;

public sealed record CompanyMemberRemoved(
    Guid CompanyId,
    Guid UserId);
