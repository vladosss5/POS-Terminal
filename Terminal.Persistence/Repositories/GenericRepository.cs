using Microsoft.EntityFrameworkCore;
using Terminal.Core.IRepositories;
using Terminal.Persistence.EventDB;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.ParamDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class GenericRepository : IGenericRepository
{
    private readonly IDbContextFactory<DataContext> _mainDbFactory;
    private readonly IDbContextFactory<ParamDbContext> _paramDbFactory;
    private readonly IDbContextFactory<EventDbContext> _eventDbFactory;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GenericRepository(
        IDbContextFactory<DataContext> mainDbFactory, 
        IDbContextFactory<ParamDbContext> paramDbFactory, 
        IDbContextFactory<EventDbContext> eventDbFactory)
    {
        _mainDbFactory = mainDbFactory;
        _paramDbFactory = paramDbFactory;
        _eventDbFactory = eventDbFactory;
    }
    
    /// <inheritdoc/>
    public async Task<List<T>> GetALotOfStringFromArbitraryTableAsync<T>(
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        string keyField,
        string dbName,
        int? lastKey = null,
        int pageSize = 300,
        CancellationToken cancellationToken = default) where T : class
    {
        await using var context = await GetDbContextAsync(dbName, cancellationToken);
        
        var query = context.Set<T>().AsNoTracking();
        
        var orderedQuery = orderBy(query);
        
        if (lastKey.HasValue)
        {
            orderedQuery = orderedQuery
                .Where(x => EF.Property<int>(x, keyField) > lastKey.Value)
                .OrderBy(x => EF.Property<int>(x, keyField));
        }
        
        var entities = await orderedQuery
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return entities;
    }

    /// <inheritdoc/>
    public async Task<int> ExecuteSqlAsync(string script, string dbName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(script))
            throw new ArgumentException("SQL script cannot be empty", nameof(script));
    
        if (string.IsNullOrWhiteSpace(dbName))
            throw new ArgumentException("Database name cannot be empty", nameof(dbName));
    
        try
        {
            await using var context = await GetDbContextAsync(dbName, cancellationToken);
        
            var rowsAffected = await context.Database.ExecuteSqlRawAsync(script, cancellationToken);
        
            return rowsAffected;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error executing SQL script on database '{dbName}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public string GetMainDbPath() => DataContext.GetDefaultDbPath();
    
    /// <summary>
    /// Получить DbContext по имени базы данных.
    /// </summary>
    /// <param name="dbName">Имя БД (DataContext, ParamDbContext, EventDbContext).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>DbContext.</returns>
    private async Task<DbContext> GetDbContextAsync(string dbName, CancellationToken cancellationToken = default)
    {
        return dbName switch
        {
            "DataContext" => await _mainDbFactory.CreateDbContextAsync(cancellationToken),
            "ParamDbContext" => await _paramDbFactory.CreateDbContextAsync(cancellationToken),
            "EventDbContext" => await _eventDbFactory.CreateDbContextAsync(cancellationToken),
            _ => throw new ArgumentException($"Unknown database name: {dbName}", nameof(dbName))
        };
    }
}