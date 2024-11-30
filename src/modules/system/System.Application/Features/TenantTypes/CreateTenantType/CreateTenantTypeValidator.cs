using FluentValidation;

namespace System.Application.Features.TenantTypes.CreateTenantType;

internal sealed class CreateTenantTypeValidator : AbstractValidator<CreateTenantTypeCommand>
{
    public CreateTenantTypeValidator()
    {
        RuleFor(p => p.Request.TenantTypeCode)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.TenantTypeName)
            .NotEmpty()
            .NotNull();
    }
}
