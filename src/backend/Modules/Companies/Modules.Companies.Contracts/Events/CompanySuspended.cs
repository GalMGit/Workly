namespace Modules.Companies.Contracts.Events;

public sealed record CompanySuspended(
    Guid CompanyId,
    string? Reason);
