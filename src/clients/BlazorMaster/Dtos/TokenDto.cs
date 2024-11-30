using System.ComponentModel.DataAnnotations;

namespace BlazorMaster.Dtos;

internal sealed record TokenRequest(string Token, string RefreshToken);

internal sealed class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool Flag { get; set; } = true;
}

internal sealed record CustomUserClaim(string Id = null!, string Username = null!, string Email = null!, string Tenant = null!, string Exp = null!);

internal sealed class LoginDto
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}