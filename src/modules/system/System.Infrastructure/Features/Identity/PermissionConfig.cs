using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class PermissionConfig : IEntityTypeConfiguration<PermissionM>
{
    public void Configure(EntityTypeBuilder<PermissionM> builder)
    {

        builder.HasKey(p => p.PermissionId);

        builder.Property(p => p.Code).HasMaxLength(20);
    }
}
