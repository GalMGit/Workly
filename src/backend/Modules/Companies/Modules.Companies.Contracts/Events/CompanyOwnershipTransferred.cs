namespace Modules.Companies.Contracts.Events;

public sealed record CompanyOwnershipTransferred(
    Guid CompanyId,
    Guid PreviousOwnerUserId,
    Guid NewOwnerUserId);
