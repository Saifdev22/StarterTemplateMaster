using Common.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCommon.Services.Contracts;

public interface IGenericService<T>
{
    Task<List<T>> GetAll(string basePath);
    Task<T> GetById(int id, Uri baseUrl);
    Task<GeneralResponse> Insert(T item, Uri baseUrl);
    Task<GeneralResponse> Update(T item, Uri baseUrl);
    Task<GeneralResponse> DeleteById(int id, Uri baseUrl);
}
