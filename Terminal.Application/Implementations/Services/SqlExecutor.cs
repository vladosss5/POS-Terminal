using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.Services;
using Terminal.Data.Context;

namespace Terminal.Application.Implementations.Services;

public class SqlExecutor : ISqlExecutor
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public SqlExecutor(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> ExecuteNonQueryAsync(string sql)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Database.ExecuteSqlRawAsync(sql);
    }
}