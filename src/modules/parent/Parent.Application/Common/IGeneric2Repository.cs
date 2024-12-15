using System.Linq.Expressions;

namespace Parent.Application.Common;

public interface IGenericRepository2<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(int id);
    Task AddAsync(TEntity entity);
    Task AddManyAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task DeleteById(int id);
    void DeleteMany(Expression<Func<TEntity, bool>> predicate);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}