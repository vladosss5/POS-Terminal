using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Entities.Models;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class ShiftRepository : IShiftRepository
{
    private readonly DataContext _context;
    
    public ShiftRepository(IDbContextFactory<DataContext> mainDbFactory)
    {
        _context = mainDbFactory.CreateDbContext();
    }
    
    /// <inheritdoc/>
    public async Task AddAsync(Shift shift)
    {
        await _context.Shifts.AddAsync(shift);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Shift shift)
    {
        _context.Shifts.Update(shift);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<ShiftStateDto?> GetLastShiftAsync(int shopKey)
    {
        var lastShiftKey = await _context.Shifts
            .Where(x => x.ShopKey == shopKey)
            .MaxAsync(x => x.ShiftKey) ?? 0;

        if (lastShiftKey == 0)
            return null;

        var shifts = await _context.Shifts
            .Where(x => x.ShopKey == shopKey && x.ShiftKey == lastShiftKey)
            .ToListAsync();

        var result = new ShiftStateDto
        {
            ShiftKey = lastShiftKey,
            OpenRecord = shifts.First(s => s.IsOpened == true),
            ClosedRecord = shifts.FirstOrDefault(s => s.IsOpened == false)
        };

        return result;
    }
}