using BlazorMaster.Dtos;
using BlazorMaster.Helpers;
using System.Net.Http.Json;

namespace BlazorMaster.Services;

internal sealed class TokenService(CustomHttpClient _httpClient) : ITokenService
{
    public const string baseUrl = "/token/accesstoken";

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

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"/token/accesstoken", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }
}