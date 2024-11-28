using Common.Domain.Jwt;

namespace System.Application.Features.Token;

public sealed record AccessTokenCommand(AccessTokenRequest Request)
    : ICommand<TokenResponse>;
