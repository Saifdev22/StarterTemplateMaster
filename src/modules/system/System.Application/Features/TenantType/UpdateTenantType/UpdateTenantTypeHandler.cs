﻿using Common.Domain.Abstractions;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.UpdateTenantType;

internal sealed class UpdateTenantTypeHandler(IGenericRepository<TenantTypeM> _repository)
        : ICommandHandler<UpdateTenantTypeCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateTenantTypeCommand request, CancellationToken cancellationToken)
    {
        TenantTypeM tenantTypeM = await _repository.GetByIdAsync(request.Request.TenantTypeId);

        tenantTypeM.TenantTypeCode = request.Request.TenantTypeCode;
        tenantTypeM.TenantTypeName = request.Request.TenantTypeName;

        _repository.Update(tenantTypeM);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}