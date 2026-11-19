using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application;
using Modules.Identity.Domain;
using Modules.Identity.Domain.Models;

namespace Modules.Identity.Infrastructure.Persistence.Context;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IdentityModuleMarker).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}