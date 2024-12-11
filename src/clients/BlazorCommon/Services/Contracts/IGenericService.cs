using Common.Domain.Results;

namespace BlazorCommon.Services.Contracts;

public interface IGenericService<TRead, TWrite>
{
    Task<Result<List<TRead>>> GetAll(string basePath);
    Task<TRead> GetById(string basePath, int id);
    Task<GeneralResponse> Insert(string basePath, TWrite item);
    Task<GeneralResponse> Update(string basePath, TWrite item);
    Task<GeneralResponse> DeleteById(Uri basePath, int id);
}
