namespace System.Domain.Features.Identity;

public class RoleM
{
    public int RoleId { get; }
    public string RoleName { get; private set; } = string.Empty;
    public string NormalizedRoleName { get; private set; } = string.Empty;
    public IReadOnlyCollection<UserRoleM> UserRoles { get; set; } = [];

    public static RoleM Create(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);

        RoleM role = new()
        {
            RoleName = roleName,
            NormalizedRoleName = roleName.ToUpperInvariant()
        };

        return role;
    }
}
