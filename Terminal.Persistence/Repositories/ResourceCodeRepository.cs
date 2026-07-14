using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class ResourceCodeRepository : IResourceCodeRepository
{
    /// <inheritdoc cref="DataContext" />
    private readonly DataContext _dataContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ResourceCodeRepository(IDbContextFactory<DataContext> mainDbFactory)
    {
        _dataContext = mainDbFactory.CreateDbContext();
    }

    /// <inheritdoc/>
    public async Task<List<ResourceCode>> GetResourceCodeCollectionAsync()
    {
        return await _dataContext.ResourceCodes.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ResourceCode>> GetShowedResourceCodesAsync()
    {
        var products = await _dataContext.ResourceCodes
            .Where(x => x.IsShow == 1)
            .OrderBy(p => p.ResourceName)
            .AsNoTracking()
            .ToListAsync();

        return products;
    }

    /// <inheritdoc/>
    public async Task<ResourceCode?> GetByResourceKeyAsync(int key)
    {
        return await _dataContext.ResourceCodes.FirstOrDefaultAsync(x => x.ResourceKey == key);
    }

    /// <inheritdoc/>
    public async Task UpdateResourceCodeAsync(ResourceCode resourceCode)
    {
        _dataContext.Update(resourceCode);
        await _dataContext.SaveChangesAsync();
    }
}