using System.Application.Common.Interfaces;
using System.Domain.Features.Token;


namespace System.Application.Features.Authentication.AccessToken;

internal sealed class AccessTokenHandler(ITokenService _tokenService)
        : ICommandHandler<AccessTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(AccessTokenCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await _tokenService.AccessToken(request.Request);

        return result;
    }
}
