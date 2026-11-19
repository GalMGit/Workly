using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Companies.Domain.Models;

namespace Modules.Companies.Infrastructure.Persistence.Configs;

public sealed class CompanyMemberConfiguration
    : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        builder.ToTable("company_members");

        builder.HasKey(x => new
        {
            x.CompanyId,
            x.UserId
        });

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .IsRequired();

        builder.Property(x => x.InvitedByUserId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.CompanyId)
            .HasFilter("\"Role\" = 'Owner'")
            .IsUnique();
    }
}
