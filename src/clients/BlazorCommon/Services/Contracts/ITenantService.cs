using Common.Domain.SharedClient;

namespace BlazorCommon.Services.Contracts;

public interface ITenantService
{
    Task<List<GetAllTenants>> GetTenants();
    Task AddCategory(CreateTenantDto tenant);
    Task UpdateCategory(CreateTenantDto tenant);
    Task DeleteCategory(int tenantId);
}