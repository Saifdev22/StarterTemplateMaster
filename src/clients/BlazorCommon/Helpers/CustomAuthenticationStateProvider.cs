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

        TokenClaimsResponse getUserClaims = GetClaimsFromToken(deserializeToken.Token!);
        if (getUserClaims == null)
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }

        // Token Expiration Validation
        // The Exp field is in Unix time
        string expiry = getUserClaims.Expiry!;
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
            TokenClaimsResponse getUserClaims = GetClaimsFromToken(session.Token!);
            claimsPrincipal = SetClaimPrincipal(getUserClaims);
        }
        else
        {
            claimsPrincipal = anonymous;
            await localStorageService.RemoveToken();
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public static ClaimsPrincipal SetClaimPrincipal(TokenClaimsResponse model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new ClaimsPrincipal
        (
            new ClaimsIdentity
            (
                [
                    new("UserId", model.UserId.ToString()!),
                    new("TenantId", model.TenantId.ToString()!),
                    new(ClaimTypes.Email, model.Email!),
                    new("TenantTypeCode", model.TenantTypeCode!),
                    new("TenantName", model.TenantName!),
                    new("DatabaseName", model.DatabaseName!),
                    new("Expiry", model.Expiry!),
                ], "JwtAuth"
            )
        );
    }

    public static TokenClaimsResponse GetClaimsFromToken(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            return new TokenClaimsResponse();
        }

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(jwtToken);
        IEnumerable<Claim> claims = token.Claims;

        string UserId = claims.First(_ => _.Type == "UserId").Value!;
        string TenantId = claims.First(_ => _.Type == "TenantId").Value!;
        string Email = claims.First(_ => _.Type == ClaimTypes.Email).Value!;
        HashSet<string> roles = new(claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
        string TenantTypeCode = claims.First(_ => _.Type == "TenantTypeCode").Value!;
        string TenantName = claims.First(_ => _.Type == "TenantName").Value!;
        string DatabaseName = claims.First(_ => _.Type == "DatabaseName").Value!;
        string Expiry = claims.First(_ => _.Type.Equals("exp", StringComparison.Ordinal)).Value;

        return new TokenClaimsResponse(int.Parse(UserId!), int.Parse(TenantId!), Email!, roles, TenantTypeCode!, TenantName, DatabaseName, Expiry);
    }

}
