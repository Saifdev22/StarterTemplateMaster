using Inventory.Infrastructure.Common.Database;
using Microsoft.EntityFrameworkCore;
using System.Domain.Features.Tenants;
using System.Infrastructure.Common.Database;

namespace Starter.API.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app, string systemDb)
    {
        ApplyMigration<SystemDbContext>(app, systemDb);

        using IServiceScope scope = app.ApplicationServices.CreateScope();
        SystemDbContext systemDbContext = scope.ServiceProvider.GetRequiredService<SystemDbContext>();

        List<TenantM> tenantsInDb = [.. systemDbContext.Tenants];
        List<TenantM> subTenantsInDb = [.. systemDbContext.Tenants];

        foreach (TenantM tenant in tenantsInDb)
        {
            ApplyMigration<InventoryDbContext>(app, tenant.ConnectionString!);
        }

        foreach (TenantM subTenant in subTenantsInDb)
        {
            ApplyMigration<InventoryDbContext>(app, subTenant.ConnectionString!);
        }

    }

    private static void ApplyMigration<TDbContext>(this IApplicationBuilder app, string connectionString)
            where TDbContext : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.SetConnectionString(connectionString);

        if (context.Database.GetPendingMigrations().Any())
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Applying Migrations for '{connectionString}' tenant.");
            Console.ResetColor();
            context.Database.Migrate();
        }

    }
}
