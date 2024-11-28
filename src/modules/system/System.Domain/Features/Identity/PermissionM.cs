namespace System.Domain.Features.Identity;

public sealed class PermissionM
{
    public int PermissionId { get; }
    public string Code { get; private set; } = string.Empty;

    public static PermissionM Create(string code)
    {
        PermissionM permission = new()
        {
            Code = code,
        };

        return permission;
    }
}