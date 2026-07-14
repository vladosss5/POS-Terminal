using Microsoft.EntityFrameworkCore;
using Terminal.Core.Interfaces;

namespace Terminal.Persistence.MainDB;

/// <inheritdoc cref="ISqlExecutor"/>
public class SqlExecutorMainDb : ISqlExecutor
{
    /// <summary>
    /// Фабрика контекста БД.
    /// </summary>
    private readonly IDbContextFactory<DataContext> _contextFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SqlExecutorMainDb(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<int> ExecuteNonQueryAsync(string sql)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Database.ExecuteSqlRawAsync(sql);
    }
}