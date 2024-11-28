using Common.Domain.Jwt;
using Common.Domain.Results;

namespace Common.Application.Authentication;

public interface ITokenService
{
    Task<Result<TokenResponse>> AccessToken(AccessTokenRequest request);
    Task<Result<TokenResponse>> RefreshToken(RefreshTokenRequest request);
}
