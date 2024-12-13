using Common.Domain.Abstractions;
using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.GetByIdTenantType;

internal sealed class GetByIdTenantTypeHandler(IGenericRepository<TenantTypeM> genericRepository)
    : IQueryHandler<GetByIdTenantTypeQuery, ReadTenantTypeDto>
{
    public async Task<Result<ReadTenantTypeDto>> Handle(
        GetByIdTenantTypeQuery request,
        CancellationToken cancellationToken)
    {
        TenantTypeM objs = await genericRepository.GetByIdAsync(request.TenantTypeId);

        ReadTenantTypeDto result = new
        (
            objs.TenantTypeId,
            objs.TenantTypeCode,
            objs.TenantTypeName
        );

        return result;
    }
}