using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class SellingRepository : ISellingRepository
{
    /// <inheritdoc cref="DataContext" />
    private readonly DataContext _dataContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SellingRepository(IDbContextFactory<DataContext> mainDbFactory)
    {
        _dataContext = mainDbFactory.CreateDbContext();
    }

    /// <inheritdoc/>
    public async Task AddAsync(Selling selling)
    {
        await _dataContext.AddAsync(selling);
        await _dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<Selling> sales)
    {
        await _dataContext.AddRangeAsync(sales);
        await _dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Selling selling)
    {
        _dataContext.Update(selling);
        await _dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<Selling?> GetSellingByCheckNumberAsync(int checkNumber)
    {
        var selling = await _dataContext.Sales.FirstOrDefaultAsync(x => x.CheckNumber == checkNumber);
        return selling;
    }
}