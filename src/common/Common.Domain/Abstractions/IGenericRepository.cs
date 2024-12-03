﻿using System.Linq.Expressions;

namespace Common.Domain.Abstractions;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    IQueryable<TEntity> GetAll(FindOptions? findOptions = null);
    TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null, CancellationToken cancellationToken = default);
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);

    //Ading
    Task AddAsync(TEntity entity);
    Task AddManyAsync(IEnumerable<TEntity> entities);

    //Update
    void Update(TEntity entity);

    //Delete
    void Delete(TEntity entity);
    void DeleteMany(Expression<Func<TEntity, bool>> predicate);

    //Other
    bool Any(Expression<Func<TEntity, bool>> predicate);
    int Count(Expression<Func<TEntity, bool>> predicate);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class FindOptions
{
    public bool IsIgnoreAutoIncludes { get; set; }
    public bool IsAsNoTracking { get; set; }
}
