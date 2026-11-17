namespace Modules.Identity.Contracts.Events;

public sealed record UserRegistrationStarted(
    string Email,
    string Username,
    string ConfirmationCode);