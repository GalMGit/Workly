namespace Modules.Identity.Contracts.Events;

public sealed record UserCreated(
    Guid UserId,
    string Username);