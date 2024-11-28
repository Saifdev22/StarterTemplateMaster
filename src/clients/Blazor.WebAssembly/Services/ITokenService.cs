using Blazor.WebAssembly.Dtos;

namespace Blazor.WebAssembly.Services;

internal interface ITokenService
{
    Task<TokenResponse> LoginUser(LoginDto request);
    Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken);
}
