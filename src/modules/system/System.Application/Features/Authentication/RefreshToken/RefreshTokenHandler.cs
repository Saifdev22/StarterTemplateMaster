using System.Application.Common.Interfaces;
using System.Domain.Features.Token;

namespace System.Application.Features.Authentication.RefreshToken;

internal sealed class RefreshTokenHandler(ITokenService _tokenService)
        : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await _tokenService.RefreshToken(request.Request);
        return result;
    }
}