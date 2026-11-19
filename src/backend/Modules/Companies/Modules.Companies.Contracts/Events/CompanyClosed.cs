namespace Modules.Companies.Contracts.Events;

public sealed record CompanyClosed(
    Guid CompanyId,
    string? Reason);
