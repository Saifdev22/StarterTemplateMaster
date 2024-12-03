using Common.Domain.SharedClient;

namespace BlazorMaster.Services.Tenant;

internal interface ITenantService
{
    Task<List<CreateTenantDto>> GetTenants();
    Task AddCategory(CreateTenantDto tenant);
    Task UpdateCategory(CreateTenantDto tenant);
    Task DeleteCategory(int tenantId);
}