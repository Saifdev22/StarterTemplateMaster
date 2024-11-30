using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantTypes.CreateTenantType;

public sealed record CreateTenantTypeCommand(CreateTenantTypeDto Request)
    : ICommand<TenantTypeM>;
