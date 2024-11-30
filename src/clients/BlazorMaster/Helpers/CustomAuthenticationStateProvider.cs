using BlazorMaster.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorMaster.Helpers;

internal sealed class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? stringToken = await localStorageService.GetToken();
        if (string.IsNullOrWhiteSpace(stringToken))
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

        TokenResponse deserializeToken = Serialization.DeserializeJsonString<TokenResponse>(stringToken);
        if (deserializeToken == null)
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

        CustomUserClaim getUserClaims = GetClaimsFromToken(deserializeToken.Token!);
        if (getUserClaims == null)
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

        // Checks the exp field of the token
        string expiry = getUserClaims.Exp;
        if (expiry == null)
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

        // The exp field is in Unix time
        DateTimeOffset datetime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry));
        if (datetime.UtcDateTime <= DateTime.UtcNow)
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

        ClaimsPrincipal claimsPrincipal = SetClaimPrincipal(getUserClaims);
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public async Task UpdateAuthenticationState(TokenResponse session)
    {
        ClaimsPrincipal claimsPrincipal;

        if (!string.IsNullOrEmpty(session.Token))
        {
            string serializeSession = Serialization.SerializeObj(session);
            await localStorageService.SetToken(serializeSession);
            CustomUserClaim getUserClaims = GetClaimsFromToken(session.Token!);
            claimsPrincipal = SetClaimPrincipal(getUserClaims);
        }
        else
        {
            claimsPrincipal = anonymous;
            await localStorageService.RemoveToken();
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public static ClaimsPrincipal SetClaimPrincipal(CustomUserClaim model)
    {
        return new ClaimsPrincipal
        (
            new ClaimsIdentity
            (
                [
                        new(ClaimTypes.NameIdentifier, model.Id),
                        new(ClaimTypes.Name, model.Username),
                        new(ClaimTypes.Email, model.Email),
                        new("tenant", model.Tenant),
                ], "JwtAuth"
            )
        );
    }

    public static CustomUserClaim GetClaimsFromToken(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            return new CustomUserClaim();
        }

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(jwtToken);
        IEnumerable<Claim> claims = token.Claims;

        string Id = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value!;
        string TenantId = claims.First(c => c.Type == "TenantId").Value!;
        string Email = claims.First(c => c.Type == ClaimTypes.Email).Value!;
        string TenantDb = claims.First(c => c.Type == "TenantDb").Value!;
        string exp = claims.First(claim => claim.Type.Equals("exp", StringComparison.Ordinal)).Value;

        return new CustomUserClaim(Id!, TenantId!, Email!, TenantDb!, exp);
    }

}
