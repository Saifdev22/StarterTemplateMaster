using BlazorCommon.Dtos;

namespace BlazorCommon.Services.Contracts;

public interface ITokenService
{
    Task<TokenResponse> LoginUser(LoginDto request);
    Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken);
}
