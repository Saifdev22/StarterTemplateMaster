using BlazorCommon.Dtos;
using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using System.Net.Http.Json;

namespace BlazorCommon.Services.Implementations;

public class TokenService(CustomHttpClient httpClient) : ITokenService
{
    public const string baseUrl = "/token/accesstoken";

    public async Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken)
    {
        HttpClient httpclient = httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{baseUrl}/refresh", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }

    public async Task<TokenResponse> LoginUser(LoginDto request)
    {
        HttpClient httpclient = httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"/token/accesstoken", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }
}