using Microsoft.EntityFrameworkCore;
using Modules.Profiles.Application;
using Modules.Profiles.Domain;

namespace Modules.Profiles.Infrastructure.Persistence.Context;

public sealed class ProfilesDbContext : DbContext
{
    public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options)
        : base(options) {}

    public DbSet<Profile> Profiles => Set<Profile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ProfilesModuleMarker).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}