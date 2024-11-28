using Blazor.WebAssembly.Dtos;
using Blazor.WebAssembly.Helpers;
using System.Net.Http.Json;

namespace Blazor.WebAssembly.Services;

internal sealed class TokenService(CustomHttpClient _httpClient) : ITokenService
{
    public const string baseUrl = "identity/tokens";

    public async Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken)
    {
        HttpClient httpclient = _httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{baseUrl}/refresh", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }

    public async Task<TokenResponse> LoginUser(LoginDto request)
    {
        HttpClient httpclient = _httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{baseUrl}/refresh", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }
}