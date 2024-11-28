using FluentValidation;

namespace System.Application.Features.Token;

internal sealed class AccessTokenValidator : AbstractValidator<AccessTokenCommand>
{
    public AccessTokenValidator()
    {
        RuleFor(p => p.Request.Email)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.Password)
            .NotEmpty()
            .NotNull();
    }
}
