using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using Common.Domain.Results;
using System.Net.Http.Json;

namespace BlazorCommon.Services.Implementations;

public class GenericService<T>(CustomHttpClient getHttpClient) : IGenericService<T>
{
    public async Task<GeneralResponse> Insert(T item, Uri baseUrl)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{baseUrl}/add", item);
        GeneralResponse? result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

    public async Task<List<T>> GetAll(string basePath)
    {
        HttpClient httpClient = getHttpClient.GetPublicHttpClient();
        List<T>? results = await httpClient.GetFromJsonAsync<List<T>>($"{basePath}/all");
        return results!;
    }

    public async Task<T> GetById(int id, Uri baseUrl)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        T? result = await httpClient.GetFromJsonAsync<T>($"{baseUrl}/single{id}");
        return result!;
    }

    public async Task<GeneralResponse> Update(T item, Uri baseUrl)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{baseUrl}/update", item);
        GeneralResponse? result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

    public async Task<GeneralResponse> DeleteById(int id, Uri baseUrl)
    {
        Uri deleteUri = new(baseUrl, $"/delete/{id}");

        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.DeleteAsync(deleteUri);
        GeneralResponse? result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

}