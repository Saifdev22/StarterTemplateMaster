using System.Domain.Features.Identity.Events;
using System.Security.Cryptography;
using System.Text;

namespace System.Domain.Features.Identity;

public class UserM : AggregateRoot
{
    public int UserId { get; }
    public int TenantId { get; private set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual IReadOnlyCollection<UserRoleM>? UserRoles { get; set; } = [];

    public static UserM Create(
        int tenantId,
        string email,
        string password)
    {
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        UserM user = new()
        {
            TenantId = tenantId,
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        user.AddDomainEvent(new UserCreatedDomainEvent(Guid.NewGuid()));

        return user;
    }

    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512 hmac = new();
        passwordSalt = hmac.Key;
        passwordHash = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(password));
    }

}

