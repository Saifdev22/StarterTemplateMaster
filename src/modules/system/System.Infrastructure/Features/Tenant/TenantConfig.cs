using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Tenants;

namespace System.Infrastructure.Features.Tenant;

internal sealed class TenantConfig : IEntityTypeConfiguration<TenantM>
{
    public void Configure(EntityTypeBuilder<TenantM> builder)
    {
        builder.HasKey(p => p.TenantId);

        builder.Property(p => p.TenantName)
            .HasMaxLength(5)
            .IsRequired();

        builder.HasIndex(p => p.DatabaseName)
            .IsUnique();

        builder.Property(p => p.DatabaseName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.ConnectionString)
            .HasMaxLength(300)
            .IsRequired();

    }
}