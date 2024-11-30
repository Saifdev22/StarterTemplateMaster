using System.Domain.Features.Token;

namespace System.Application.Features.Token;

public sealed record AccessTokenCommand(AccessTokenRequest Request)
    : ICommand<TokenResponse>;
