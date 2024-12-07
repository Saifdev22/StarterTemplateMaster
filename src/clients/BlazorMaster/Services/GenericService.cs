using System.Net.Http.Json;

namespace BlazorMaster.Services;

internal class GenericService<T>(HttpClient httpClient, string apiUrl) : IGenericService<T> where T : class
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiUrl = apiUrl;

    public async Task<T> CreateAsync(T entity)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_apiUrl, entity);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        return null;
    }

    public async Task<List<T>> GetAllAsync()
    {
        List<T>? response = await _httpClient.GetFromJsonAsync<List<T>>(_apiUrl);
        return response ?? [];
    }

    public async Task<T> GetByIdAsync(int id)
    {
        T? response = await _httpClient.GetFromJsonAsync<T>($"{_apiUrl}/{id}");
        return response;
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{id}", entity);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Construct the Uri
        Uri requestUri = new($"{_apiUrl}/{id}");

        // Send DELETE request using the Uri
        var response = await _httpClient.DeleteAsync(requestUri);

        // Return true if the response is successful
        return response.IsSuccessStatusCode;
    }
}