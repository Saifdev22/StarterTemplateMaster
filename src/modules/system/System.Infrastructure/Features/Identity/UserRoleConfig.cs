using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class UserRoleConfig : IEntityTypeConfiguration<UserRoleM>
{
    public void Configure(EntityTypeBuilder<UserRoleM> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

        builder
        .HasOne(ur => ur.Role)
        .WithMany(u => u.UserRoles)
        .HasForeignKey(ur => ur.RoleId);

    }

}
