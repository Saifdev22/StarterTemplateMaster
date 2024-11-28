using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class RoleConfig : IEntityTypeConfiguration<RoleM>
{
    public void Configure(EntityTypeBuilder<RoleM> builder)
    {
        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleName)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(r => r.RoleName)
            .IsUnique();

        builder.Property(r => r.NormalizedRoleName)
            .HasMaxLength(20)
            .IsRequired();
    }
}
