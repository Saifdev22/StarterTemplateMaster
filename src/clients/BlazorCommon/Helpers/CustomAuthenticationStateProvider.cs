using BlazorCommon.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorCommon.Helpers;

public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
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

        // Token Expiration Validation
        // The Exp field is in Unix time
        string expiry = getUserClaims.Exp;
        if (expiry == null)
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

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

        if (session == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(session.Token) || !string.IsNullOrEmpty(session.RefreshToken))
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
        ArgumentNullException.ThrowIfNull(model);

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

        string Id = claims.First(_ => _.Type == ClaimTypes.NameIdentifier).Value!;
        string TenantId = claims.First(_ => _.Type == "TenantId").Value!;
        string Email = claims.First(_ => _.Type == ClaimTypes.Email).Value!;
        string TenantDb = claims.First(_ => _.Type == "TenantDb").Value!;
        string exp = claims.First(_ => _.Type.Equals("exp", StringComparison.Ordinal)).Value;

        return new CustomUserClaim(Id!, TenantId!, Email!, TenantDb!, exp);
    }

}
