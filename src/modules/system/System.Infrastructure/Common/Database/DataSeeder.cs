using System.Domain.Features.Tenant;
using System.Domain.Identity;

namespace System.Infrastructure.Common.Database;

public class DataSeeder(SystemDbContext context)
{
    private readonly SystemDbContext _context = context;

    public void Seed()
    {
        SeedRoles();
        SeedUsers();
        SeedPermissions();
        SeedUserRoles();
        SeedRolePermissions();
        SeedTenantType();
        SeedTenants();
        SeedTenantUsers();
    }

    public void SeedRoles()
    {
        if (!_context.Roles.Any())
        {
            RoleM adminRole = RoleM.Create("Admin");
            RoleM userRole = RoleM.Create("User");

            _context.Roles.AddRange(adminRole, userRole);
            _context.SaveChanges();
        }
    }

    public void SeedUsers()
    {
        if (!_context.Users.Any())
        {
            UserM user1 = UserM.Create("admin@gmail.com", "12345678");
            UserM user2 = UserM.Create("user@gmail.com", "12345678");

            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();
        }
    }

    public void SeedPermissions()
    {
        if (!_context.Permissions.Any())
        {
            PermissionM permission1 = PermissionM.Create("inventory:view");
            PermissionM permission2 = PermissionM.Create("system:view");

            _context.Permissions.AddRange(permission1, permission2);
            _context.SaveChanges();
        }
    }

    public void SeedUserRoles()
    {
        if (!_context.UserRoles.Any())
        {
            _context.UserRoles.AddRange
            (
                new UserRoleM { UserId = 1, RoleId = 1 },
                new UserRoleM { UserId = 2, RoleId = 2 }
            );
            _context.SaveChanges();
        }
    }

    public void SeedRolePermissions()
    {
        if (!_context.RolePermissions.Any())
        {
            _context.RolePermissions.AddRange
            (
                new RolePermissionM { RoleId = 1, PermissionId = 1 },
                new RolePermissionM { RoleId = 2, PermissionId = 2 }
            );
            _context.SaveChanges();
        }
    }

    public void SeedTenantType()
    {
        if (!_context.TenantTypes.Any())
        {
            TenantTypeM tenantType1 = TenantTypeM.Create("HO", "Head Office");
            TenantTypeM tenantType2 = TenantTypeM.Create("ST", "Store");

            _context.TenantTypes.AddRange(tenantType1, tenantType2);
            _context.SaveChanges();
        }
    }

    public void SeedTenants()
    {
        if (!_context.Tenants.Any())
        {
            TenantM tenant1 = TenantM.Create
            (
                1,
                0,
                "TenantName1",
                "Tenant-DB-1",
                "Server=102.214.10.24,1433;Database=Tenant-DB-1;User Id=sa;Password=25122000SK;Encrypt=False;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=False"
            );

            _context.Tenants.AddRange(tenant1);
            _context.SaveChanges();
        }
    }

    public void SeedTenantUsers()
    {
        if (!_context.TenantUsers.Any())
        {
            _context.TenantUsers.AddRange
            (
                new TenantUsersM { TenantId = 1, UserId = 1 },
                new TenantUsersM { TenantId = 1, UserId = 2 }
            );
            _context.SaveChanges();
        }
    }
}