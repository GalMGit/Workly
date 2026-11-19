namespace Modules.Companies.Contracts.Events;

public sealed record CompanyInvitationAccepted(
    Guid CompanyId,
    Guid UserId,
    string Role);
