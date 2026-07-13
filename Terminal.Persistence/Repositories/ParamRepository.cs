using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.ParamDb;
using Terminal.Core.Enums;
using Terminal.Core.IRepositories;
using Terminal.Persistence.ParamDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class ParamRepository : IParamRepository
{
    /// <inheritdoc cref="ParamDbContext" />
    private readonly ParamDbContext _paramDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="paramDbFactory"></param>
    public ParamRepository(IDbContextFactory<ParamDbContext> paramDbFactory)
    {
        _paramDbContext = paramDbFactory.CreateDbContext();
    }

    /// <inheritdoc/>
    public async Task AddAsync(Param param)
    {
        await _paramDbContext.AddAsync(param);
        await _paramDbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Param param)
    {
        _paramDbContext.Update(param);
        await _paramDbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<Param?> GetByNameAsync(AppParameter name)
    {
        return await _paramDbContext.Params.FirstOrDefaultAsync(x => x.Name == name.ToString());
    }
}