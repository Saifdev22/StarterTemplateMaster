namespace BlazorMaster.Services;

internal interface IGenericService<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> UpdateAsync(int id, T entity);
    Task<bool> DeleteAsync(int id);
}
