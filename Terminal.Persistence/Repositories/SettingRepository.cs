using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class SettingRepository : ISettingRepository
{
    /// <inheritdoc cref="DataContext" />
    private readonly DataContext _context;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingRepository(IDbContextFactory<DataContext> mainDbFactory)
    {
        _context = mainDbFactory.CreateDbContext();
    }
    
    /// <inheritdoc/>
    public async Task<Setting?> GetByKeyAsync(SettingsKey key)
    {
        return await _context.Settings.FindAsync(key);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Setting setting)
    {
        _context.Update(setting);
        await _context.SaveChangesAsync();
    }
}