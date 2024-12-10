using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using Common.Domain.SharedClient;
using System.Net.Http.Json;

namespace BlazorCommon.Services.Implementations;

public sealed class TenantService(HttpClient http) : ITenantService
{
    public Task AddCategory(CreateTenantDto tenant)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategory(int tenantId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GetAllTenants>> GetTenants()
    {
        List<GetAllTenants>? response = await http.GetFromJsonAsync<List<GetAllTenants>>("tenant/all");

        return response!;
    }

    public Task UpdateCategory(CreateTenantDto tenant)
    {
        throw new NotImplementedException();
    }
}
