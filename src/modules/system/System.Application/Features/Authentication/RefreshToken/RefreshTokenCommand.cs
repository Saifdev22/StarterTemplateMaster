using System.Domain.Features.Token;

namespace System.Application.Features.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(RefreshTokenRequest Request)
    : ICommand<TokenResponse>;