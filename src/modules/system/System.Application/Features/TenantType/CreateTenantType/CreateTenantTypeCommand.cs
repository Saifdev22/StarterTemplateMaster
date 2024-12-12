using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.CreateTenantType;

public sealed record CreateTenantTypeCommand(WriteTenantTypeDto Request)
    : ICommand<TenantTypeM>;
