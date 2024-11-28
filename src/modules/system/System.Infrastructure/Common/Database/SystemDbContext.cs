using Common.Infrastructure.Database;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Application.Common.Interfaces;
using System.Domain.Features.Identity;
using System.Domain.Features.Tenants;
using System.Reflection;

namespace System.Infrastructure.Common.Database;

public sealed class SystemDbContext(DbContextOptions<SystemDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<TenantTypeM> TenantTypes => Set<TenantTypeM>();
    public DbSet<TenantM> Tenants => Set<TenantM>();
    internal DbSet<UserM> Users => Set<UserM>();
    internal DbSet<RoleM> Roles => Set<RoleM>();
    internal DbSet<PermissionM> Permissions => Set<PermissionM>();
    internal DbSet<UserRoleM> UserRoles => Set<UserRoleM>();
    internal DbSet<RolePermissionM> RolePermissions => Set<RolePermissionM>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Users);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        //DbInitializer.Seed(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        optionsBuilder.AddInterceptors(new AuditableEntityInterceptor());
    }
}
