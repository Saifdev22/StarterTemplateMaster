using Common.Application.Authentication;
using Common.Domain.Jwt;


namespace System.Application.Features.Token;

internal sealed class AccessTokenHandler(ITokenService _tokenService)
        : ICommandHandler<AccessTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(AccessTokenCommand request, CancellationToken cancellationToken)
    {
        return await _tokenService.AccessToken(request.Request);
    }
}
