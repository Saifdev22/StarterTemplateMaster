using Common.Domain.Abstractions;
using System.Domain.Features.Tenant;

namespace System.Application.Features.Tenant.GetAllTenant;

internal sealed class GetAllTenantHandler(IGenericRepository<TenantM> genericRepository)
    : IQueryHandler<GetAllTenantQuery, List<TenantM>>
{
    public async Task<Result<List<TenantM>>> Handle(
        GetAllTenantQuery request,
        CancellationToken cancellationToken)
    {
        List<TenantM> tenants = await genericRepository.GetAllAsync();

        return tenants;
    }
}