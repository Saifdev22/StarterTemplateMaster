using Inventory.Infrastructure.Common.Database;
using Microsoft.EntityFrameworkCore;
using System.Domain.Features.Tenant;
using System.Infrastructure.Common.Database;

namespace Starter.API.Extensions;

internal static class MigrationExtensions
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        await ApplyMigration2(app);
        ApplyMigration<SystemDbContext>(app, null);

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

    private static async Task ApplyMigration2(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope(); // Corrected this line
                                                                           // Get the DatabaseInitializer service from the service provider
        DatabaseInitializer databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        // Initialize the database
        await databaseInitializer.Execute(); // Initialize the database
    }

    private static void ApplyMigration<TDbContext>(this IApplicationBuilder app, string? connectionString)
            where TDbContext : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        if (connectionString != null)
        {
            context.Database.SetConnectionString(connectionString);
        }

        if (context.Database.GetPendingMigrations().Any())
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Applying Migrations for '{connectionString}' tenant.");
            Console.ResetColor();
            context.Database.Migrate();
        }
    }
}
