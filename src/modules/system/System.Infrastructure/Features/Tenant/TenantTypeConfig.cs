using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Tenant;

namespace System.Infrastructure.Features.Tenant;

internal sealed class TenantTypeConfig : IEntityTypeConfiguration<TenantTypeM>
{
    public void Configure(EntityTypeBuilder<TenantTypeM> builder)
    {
        builder.HasKey(p => p.TenantTypeId);

        builder
            .HasMany(p => p.Tenants)
            .WithOne(p => p.TenantType);

        builder.Property(p => p.TenantTypeCode)
            .HasMaxLength(5)
            .IsRequired();

        builder.HasIndex(p => p.TenantTypeCode)
                .IsUnique();

    }

}
