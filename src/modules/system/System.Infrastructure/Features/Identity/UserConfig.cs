using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class UserConfig : IEntityTypeConfiguration<UserM>
{
    public void Configure(EntityTypeBuilder<UserM> builder)
    {
        builder.HasKey(p => p.UserId);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.HasIndex(p => p.Email)
            .IsUnique();

        builder.Property(p => p.Email)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.PasswordHash)
            .IsRequired();

        builder.Property(p => p.PasswordHash)
            .IsRequired();
    }
}