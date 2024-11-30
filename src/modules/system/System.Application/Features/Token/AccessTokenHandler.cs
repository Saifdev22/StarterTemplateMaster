using System.Application.Common.Interfaces;
using System.Domain.Features.Token;


namespace System.Application.Features.Token;

internal sealed class CreateTenantHandler(ITokenService _tokenService)
        : ICommandHandler<AccessTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(AccessTokenCommand request, CancellationToken cancellationToken)
    {
        return await _tokenService.AccessToken(request.Request);
    }
}
