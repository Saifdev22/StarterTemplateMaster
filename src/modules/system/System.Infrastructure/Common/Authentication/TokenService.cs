using Common.Application.Authentication;
using Common.Domain.Abstractions;
using Common.Domain.Errors;
using Common.Domain.Jwt;
using Common.Domain.Results;
using Common.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Application.Common.Interfaces;
using System.Domain.Features.Tenant;
using System.Domain.Features.Token;
using System.Domain.Identity;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Infrastructure.Common.Database;
using System.Security.Claims;
using System.Text;

namespace System.Infrastructure.Common.Authentication;

public class TokenService(
    IOptions<JwtOptions> JwtOptions,
    IGenericRepository<UserM> Repository,
    SystemDbContext SystemContext) : ITokenService
{
    public async Task<Result<TokenResponse>> AccessToken(AccessTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        UserM userDto = Repository.FindOne(p => p.Email == request.Email);

        if (userDto is null)
        {
            return Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "User not found."));
        }

        TenantM? tenant = await SystemContext.Tenants.FirstOrDefaultAsync(t => t.TenantId == 1);

        return tenant switch
        {
            null => Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "Tenant not found.")),
            _ => !IdentityMethodExtensions.VerifyPasswordHash(request.Password, userDto.PasswordHash, userDto.PasswordSalt)
                                ? Result.Failure<TokenResponse>(CustomError.Conflict("TokenService", "Invalid Credentials."))
                                : await GenerateTokensAndUpdateUser(userDto, tenant),
        };
    }

    public async Task<Result<TokenResponse>> RefreshToken(RefreshTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ClaimsPrincipal userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        UserM user = Repository.FindOne(p => p.Email == userPrincipal.GetUserEmail());

        if (user is null)
        {
            return Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "User not found."));
        }

        TenantM? tenant = await SystemContext.Tenants.FirstOrDefaultAsync(t => t.TenantId == 1);
        ArgumentNullException.ThrowIfNull(tenant);

        return user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow
            ? Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "Invalid Refresh Token."))
            : await GenerateTokensAndUpdateUser(user, tenant);
    }

    private async Task<Result<TokenResponse>> GenerateTokensAndUpdateUser(UserM user, TenantM tenant)
    {
        CustomUserClaim userClaims = new(user.UserId, user.Email!, tenant.TenantId, tenant.DatabaseName);
        string token = GenerateJwt(userClaims);

        user.RefreshToken = IdentityMethodExtensions.GenerateRefreshToken();
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(JwtOptions.Value.RefreshTokenExpirationInDays);

        Repository.Update(user);
        await Repository.SaveChangesAsync();

        return new TokenResponse()
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiration
        };
    }

    private string GenerateJwt(CustomUserClaim customClaims)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(customClaims));
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        JwtSecurityToken token = new(
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(JwtOptions.Value.TokenExpirationInMinutes),
             signingCredentials: signingCredentials,
             issuer: JwtOptions.Value.Issuer,
             audience: JwtOptions.Value.Audience
                                                                        );
        JwtSecurityTokenHandler tokenHandler = new();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(JwtOptions.Value.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private static List<Claim> GetClaims(CustomUserClaim customClaims)
    {
        List<Claim> claims =
        [
                new Claim(ClaimTypes.NameIdentifier, customClaims.Id.ToString()),
                new Claim(ClaimTypes.Email, customClaims.Email),
                new Claim("TenantId", customClaims.TenantId.ToString(CultureInfo.InvariantCulture)),
                new Claim("TenantDb", customClaims.TenantDb)
        ];

        //if (customClaims.Roles != null)
        //{
        //    foreach (string role in customClaims.Roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }
        //}

        return claims;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Value.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = JwtOptions.Value.Audience,
            ValidIssuer = JwtOptions.Value.Issuer,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken? securityToken);

        return securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase)
            ? throw new SecurityTokenValidationException("Invalid token.")
            : principal;
    }

}

public record CustomUserClaim(int Id, string Email, int TenantId, string TenantDb);
