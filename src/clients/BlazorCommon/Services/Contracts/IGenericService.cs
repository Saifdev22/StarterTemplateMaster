using Common.Domain.Results;

namespace BlazorCommon.Services.Contracts;

public interface IGenericService<T>
{
    Task<List<T>> GetAll(string basePath);
    Task<T> GetById(int id, Uri baseUrl);
    Task<GeneralResponse> Insert(T item, Uri baseUrl);
    Task<GeneralResponse> Update(T item, Uri baseUrl);
    Task<GeneralResponse> DeleteById(int id, Uri baseUrl);
}
