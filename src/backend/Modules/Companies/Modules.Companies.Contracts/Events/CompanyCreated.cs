namespace Modules.Companies.Contracts.Events;

public sealed record CompanyCreated(
    Guid CompanyId,
    Guid OwnerUserId,
    string Name,
    string Slug);
