namespace Modules.Profiles.Contracts.Events;

public sealed record UserProfileCreated(
    Guid UserId,
    string Username);