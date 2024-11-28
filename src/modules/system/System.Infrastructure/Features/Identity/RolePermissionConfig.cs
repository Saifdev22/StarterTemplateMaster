using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class RolePermissionConfig : IEntityTypeConfiguration<RolePermissionM>
{
    public void Configure(EntityTypeBuilder<RolePermissionM> builder)
    {
        builder
            .HasKey(ur => new { ur.RoleId, ur.PermissionId });

        builder
            .HasOne<PermissionM>()
            .WithMany()
            .HasForeignKey(ur => ur.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<RoleM>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}

