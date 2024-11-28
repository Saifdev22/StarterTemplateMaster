namespace System.Domain.Features.Identity;

public sealed class UserRoleM
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public UserM? User { get; set; }
    public RoleM? Role { get; set; }
}