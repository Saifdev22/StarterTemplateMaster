using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using Common.Domain.Results;
using System.Net.Http.Json;

namespace BlazorCommon.Services.Implementations;

public class GenericService<TRead, TWrite>(CustomHttpClient getHttpClient) : IGenericService<TRead, TWrite>
{
    public async Task<GeneralResponse> Insert(string basePath, TWrite item)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{basePath}/create", item);
        GeneralResponse? result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

    public async Task<Result<List<TRead>>> GetAll(string basePath)
    {
        HttpClient httpClient = getHttpClient.GetPublicHttpClient();
        List<TRead>? results = await httpClient.GetFromJsonAsync<List<TRead>>($"{basePath}/all");
        return results!;
    }

    public async Task<TRead> GetById(string basePath, int id)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        TRead? result = await httpClient.GetFromJsonAsync<TRead>($"{basePath}/single/{id}");
        return result!;
    }

    public async Task<GeneralResponse> Update(string basePath, TWrite item)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{basePath}/update", item);
        GeneralResponse? result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

    public async Task<GeneralResponse> DeleteById(Uri basePath, int id)
    {
        Uri deleteUri = new(basePath, $"/delete/{id}");

        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.DeleteAsync(deleteUri);
        GeneralResponse? result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

}