namespace BlazorCommon.Services.Contracts;

public interface IGenericService<TRead, TWrite>
{
    Task<Result<List<TRead>>> GetAll(string basePath);
    Task<TRead> GetById(string basePath, int id);
    Task<Result<bool>> Insert(string basePath, TWrite item);
    Task<Result<bool>> Update(string basePath, TWrite item);
    Task<Result<bool>> DeleteById(Uri basePath, int id);
}
