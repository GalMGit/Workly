using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Companies.Domain.Models;

namespace Modules.Companies.Infrastructure.Persistence.Configs;

public sealed class CompanyConfiguration
    : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.LogoPath)
            .HasMaxLength(512);

        builder.Property(x => x.WebsiteUrl)
            .HasMaxLength(512);

        builder.Property(x => x.Industry)
            .HasMaxLength(100);

        builder.Property(x => x.Size)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(256);

        builder.Property(x => x.ContactPhone)
            .HasMaxLength(32);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.StatusReason)
            .HasMaxLength(500);

        builder.Property(x => x.VerifiedAt);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.OwnerUserId);

        builder.HasMany(x => x.Members)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Invitations)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
