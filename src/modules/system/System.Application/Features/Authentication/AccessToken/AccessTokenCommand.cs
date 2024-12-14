using System.Domain.Features.Token;

namespace System.Application.Features.Authentication.AccessToken;

public sealed record AccessTokenCommand(AccessTokenRequest Request)
    : ICommand<TokenResponse>;
