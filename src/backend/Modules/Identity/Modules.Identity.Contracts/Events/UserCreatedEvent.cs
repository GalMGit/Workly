namespace Modules.Identity.Contracts.Events;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Username);