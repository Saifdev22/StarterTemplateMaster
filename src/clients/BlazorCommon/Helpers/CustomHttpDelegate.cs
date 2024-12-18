using BlazorCommon.Services.Contracts;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorCommon.Helpers;

public sealed class CustomHttpDelegate(LocalStorageService localStorageService, ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login", StringComparison.OrdinalIgnoreCase);
        bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("accesstoken", StringComparison.OrdinalIgnoreCase);
        bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token", StringComparison.OrdinalIgnoreCase);

        if (loginUrl || registerUrl || refreshTokenUrl)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            string? stringToken = await localStorageService.GetToken();
            if (stringToken == null)
            {
                return result;
            }

            string token = string.Empty;
            try
            {
                token = request.Headers.Authorization!.Parameter!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            TokenResponse? deserializedToken = Serialization.DeserializeJsonString<TokenResponse>(stringToken);
            if (deserializedToken is null)
            {
                return result;
            }

            if (string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                return await base.SendAsync(request, cancellationToken);
            }

            // Call for refresh token.
            Result<TokenResponse> newJwtToken = await GetRefreshToken(deserializedToken, cancellationToken);
            if (string.IsNullOrEmpty(newJwtToken.Value.Token))
            {
                return result;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwtToken.Value.Token);

            return await base.SendAsync(request, cancellationToken);
        }

        return result;
    }

    private async Task<Result<TokenResponse>> GetRefreshToken(TokenResponse tokens, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await tokenService.GetTokenWithRefreshToken(new TokenRequest(tokens.Token, tokens.RefreshToken), "N/A", cancellationToken);
        string serializedToken = Serialization.SerializeObj(new TokenResponse() { Token = result.Value.Token, RefreshToken = result.Value.RefreshToken, RefreshTokenExpiryTime = result.Value.RefreshTokenExpiryTime });
        await localStorageService.SetToken(serializedToken);
        return result!;
    }

}