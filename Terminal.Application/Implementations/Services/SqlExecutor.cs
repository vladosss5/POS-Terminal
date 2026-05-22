using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.Services;
using Terminal.Persistence.MainDB;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc cref="ISqlExecutor"/>
public class SqlExecutor : ISqlExecutor
{
    /// <summary>
    /// Фабрика контекста БД.
    /// </summary>
    private readonly IDbContextFactory<DataContext> _contextFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SqlExecutor(IDbContextFactory<DataContext> contextFactory)
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