﻿using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace System.Infrastructure.Common.Database;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;
    private readonly SystemDbContext _dbContext;

    public GenericRepository(SystemDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddManyAsync(IEnumerable<TEntity> entities)
    {
        await _dbContext.Set<TEntity>().AddRangeAsync(entities);
    }

    public void Delete(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public void DeleteMany(Expression<Func<TEntity, bool>> predicate)
    {
        IQueryable<TEntity> entities = Find(predicate);
        _dbContext.Set<TEntity>().RemoveRange(entities);
    }

    public TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null, CancellationToken cancellationToken = default)
    {
        return Get(findOptions).FirstOrDefault(predicate)!;
    }

    public Task<TEntity?> FindOne(Func<TEntity, bool> value, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)
    {
        return Get(findOptions).Where(predicate);
    }

    public IQueryable<TEntity> GetAll(FindOptions? findOptions = null)
    {
        return Get(findOptions);
    }
    public void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbContext.Set<TEntity>().Any(predicate);
    }

    public int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbContext.Set<TEntity>().Count(predicate);
    }

    private DbSet<TEntity> Get(FindOptions? findOptions = null)
    {
        findOptions ??= new FindOptions();
        DbSet<TEntity> entity = _dbContext.Set<TEntity>();
        if (findOptions.IsAsNoTracking && findOptions.IsIgnoreAutoIncludes)
        {
            entity.IgnoreAutoIncludes().AsNoTracking();
        }
        else if (findOptions.IsIgnoreAutoIncludes)
        {
            entity.IgnoreAutoIncludes();
        }
        else if (findOptions.IsAsNoTracking)
        {
            entity.AsNoTracking();
        }
        return entity;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

}
