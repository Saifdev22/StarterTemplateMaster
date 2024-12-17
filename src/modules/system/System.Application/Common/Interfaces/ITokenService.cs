using Common.Domain.TransferObjects.System;
using System.Domain.Features.Token;

namespace System.Application.Common.Interfaces;

public interface ITokenService
{
    Task<Result<TokenResponse>> AccessToken(AccessTokenRequest request);
    Task<Result<TokenResponse>> RefreshToken(RefreshTokenRequest request);
    Task<TokenClaimsResponse> GetUserClaims(string email);
}
