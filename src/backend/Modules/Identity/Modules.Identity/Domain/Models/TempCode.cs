namespace Modules.Identity.Domain.Models;

public sealed class TempCode
{
    public string ConfirmationCode { get; set; } = null!;
    public DateTime CodeExpiresAt { get; set; }
}