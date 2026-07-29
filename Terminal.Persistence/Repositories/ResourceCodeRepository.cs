using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class ResourceCodeRepository : IResourceCodeRepository
{
    /// <inheritdoc cref="DataContext" />
    // private readonly DataContext _dataContext;

    private readonly IDbContextFactory<DataContext> _mainDbFactory;

    private readonly ILogger<ResourceCodeRepository> _logger;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ResourceCodeRepository(
        IDbContextFactory<DataContext> mainDbFactory, 
        ILogger<ResourceCodeRepository> logger)
    {
        _logger = logger;
        _mainDbFactory = mainDbFactory;
        // _dataContext = mainDbFactory.CreateDbContext();
    }

    /// <inheritdoc/>
    public async Task<List<ResourceCode>> GetResourceCodeCollectionAsync()
    {
        var dataContext = await _mainDbFactory.CreateDbContextAsync();
        return await dataContext.ResourceCodes.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ResourceCode>> GetShowedResourceCodesAsync()
    {
        var dataContext = await _mainDbFactory.CreateDbContextAsync();
        var products = await dataContext.ResourceCodes
            .Where(x => x.IsShow == 1)
            .OrderBy(p => p.ResourceName)
            .AsNoTracking()
            .ToListAsync();

        return products;
    }

    /// <inheritdoc/>
    public async Task<ResourceCode?> GetByResourceKeyAsync(int key)
    {
        _logger.LogInformation($"Resource repository: GetByResourceKeyAsync where key={key}");
        
        var dataContext = await _mainDbFactory.CreateDbContextAsync();
        var resource = await dataContext.ResourceCodes.FirstOrDefaultAsync(x => x.ResourceKey == key);
        
        _logger.LogInformation(resource != null
            ? $"Resource repository: Resource with key {key} found"
            : $"Resource repository: Resource with key {key} not found");

        return resource;
    }

    /// <inheritdoc/>
    public async Task UpdateResourceCodeAsync(ResourceCode resourceCode)
    {
        var dataContext = await _mainDbFactory.CreateDbContextAsync();
        dataContext.Update(resourceCode);
        await dataContext.SaveChangesAsync();
    }
}