namespace Modules.Identity.Domain;

public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Permission> Permissions { get; set; } = [];
}