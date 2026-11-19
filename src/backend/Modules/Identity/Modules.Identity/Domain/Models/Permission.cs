namespace Modules.Identity.Domain.Models;

public sealed class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}