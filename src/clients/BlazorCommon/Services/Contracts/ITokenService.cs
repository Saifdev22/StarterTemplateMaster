namespace BlazorCommon.Services.Contracts;

public interface ITokenService
{
    Task<Result<TokenResponse>> LoginUser(LoginDto request);
    Task<Result<TokenResponse>> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken);
}
