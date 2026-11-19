using Microsoft.EntityFrameworkCore;
using Modules.Companies.Application;
using Modules.Companies.Domain.Models;

namespace Modules.Companies.Infrastructure.Persistence.Context;

public sealed class CompaniesDbContext(
    DbContextOptions<CompaniesDbContext> options)
    : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
    public DbSet<CompanyInvitation> CompanyInvitations => Set<CompanyInvitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CompaniesModuleMarker).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
