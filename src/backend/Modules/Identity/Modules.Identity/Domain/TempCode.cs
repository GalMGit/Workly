namespace Modules.Identity.Domain;

public sealed class TempCode
{
    public string ConfirmationCode { get; set; } = null!;
    public DateTime CodeExpiresAt { get; set; }
}