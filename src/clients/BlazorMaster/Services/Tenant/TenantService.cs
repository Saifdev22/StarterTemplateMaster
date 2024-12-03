using Common.Domain.SharedClient;
using System.Net.Http.Json;

namespace BlazorMaster.Services.Tenant;

internal sealed class TenantService(HttpClient _http) : ITenantService
{
    public Task AddCategory(CreateTenantDto tenant)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategory(int tenantId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CreateTenantDto>> GetTenants()
    {
        List<CreateTenantDto>? response = await _http.GetFromJsonAsync<List<CreateTenantDto>>("tenant/all");

        return response!;
    }

    public Task UpdateCategory(CreateTenantDto tenant)
    {
        throw new NotImplementedException();
    }
}
