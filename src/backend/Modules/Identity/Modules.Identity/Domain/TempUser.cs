namespace Modules.Identity.Domain;

public sealed class TempUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string ConfirmationCode { get; set; } = null!;
    public DateTime CodeExpiresAt { get; set; }
    public ICollection<Role> Roles { get; set; } = [];
}