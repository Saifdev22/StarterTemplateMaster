using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using System.Net.Http.Json;

namespace BlazorCommon.Services.Implementations;

public class TokenService(CustomHttpClient httpClient) : ITokenService
{
    public const string baseUrl = "/token/accesstoken";

    public async Task<Result<TokenResponse>> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken)
    {
        HttpClient httpclient = httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{Constants.TokenBaseUrl}/refresh", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }

    public async Task<Result<TokenResponse>> LoginUser(LoginDto request)
    {
        HttpClient httpclient = httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{Constants.TokenBaseUrl}/accesstoken", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return result!;
    }
}