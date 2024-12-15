using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Domain.Identity;
using System.Domain.Features.Tenant;

namespace System.Infrastructure.Features.Multitenancy;

internal sealed class TenantUsersConfig : IEntityTypeConfiguration<TenantUsersM>
{
    public void Configure(EntityTypeBuilder<TenantUsersM> builder)
    {
        builder.HasKey(_ => new { _.TenantId, _.UserId });

        builder.HasOne<TenantM>()
               .WithMany()
               .HasForeignKey(_ => _.TenantId);

        builder.HasOne<UserM>()
               .WithMany()
               .HasForeignKey(_ => _.UserId);
    }
}