using Common.Application.Database;
using Common.Infrastructure.System;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Common.Infrastructure.Database;

internal sealed class DbConnectionFactory(CurrentTenant ct) : IDbConnectionFactory
{
    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null!)
    {
        using DbConnection connection = GetConnection();
        await connection.OpenAsync();

        Task<T> result = connection.QueryFirstOrDefaultAsync<T>(sql, parameters)!;
        return await result!;
    }

    public async Task<List<T>> QueryAsync<T>(string sql, object parameters = null!, bool systemDb = false)
    {
        using DbConnection connection = GetConnection(systemDb);
        await connection.OpenAsync();

        IEnumerable<T> result = await connection.QueryAsync<T>(sql, parameters);
        return result.ToList();
    }

    public async Task<int> ExecuteAsync(string sql, object parameters = null!)
    {
        using DbConnection connection = GetConnection();
        await connection.OpenAsync();

        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null!)
    {
        using DbConnection connection = GetConnection();
        await connection.OpenAsync();

        Task<T> result = connection.QuerySingleOrDefaultAsync<T>(sql, parameters)!;
        return await result;
    }

    public DbConnection GetConnection(bool systemDb = false)
    {
        return systemDb
            ? new SqlConnection(ct.GetDefaultConnectionstring())
            : new SqlConnection(ct.GetConnectionString());
    }

    public async ValueTask<DbConnection> OpenConnectionAsync(string? connectionString = null)
    {
        SqlConnection connection = !string.IsNullOrEmpty(connectionString) ?
            new SqlConnection(connectionString) :
            new SqlConnection(ct.GetConnectionString());

        await connection.OpenAsync();
        return connection;
    }

    public async ValueTask<DbConnection> OpenParentConnectionAsync(string? connectionString = null)
    {
        SqlConnection connection = new(ct.GetTenantConnectionString());

        await connection.OpenAsync();
        return connection;
    }
}
