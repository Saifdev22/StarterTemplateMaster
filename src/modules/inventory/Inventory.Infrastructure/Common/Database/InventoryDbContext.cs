using Common.Infrastructure.Database;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Inventory.Application.Common;
using Inventory.Domain.Features.CategoryGroup;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Inventory.Infrastructure.Common.Database;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<CategoryGroupM> Tenants => Set<CategoryGroupM>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Inventory);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        optionsBuilder.AddInterceptors(new AuditableEntityInterceptor());
    }

}
