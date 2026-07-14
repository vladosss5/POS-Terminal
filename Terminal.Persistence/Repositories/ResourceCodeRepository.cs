using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

public class ResourceCodeRepository : IResourceCodeRepository
{
    private readonly DataContext _dataContext;
    
    public ResourceCodeRepository(IDbContextFactory<DataContext> mainDbFactory)
    {
        _dataContext = mainDbFactory.CreateDbContext();
    }
    
    public async Task<List<ResourceCode>> GetShowedResourceCodesAsync()
    {
        var products = await _dataContext.ResourceCodes
            .Where(x => x.IsShow == 1)
            .OrderBy(p => p.ResourceName)
            .AsNoTracking()
            .ToListAsync();

        return products;
    }
}