using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Persistence.MainDB;

namespace Terminal.Application.Implementations.DbEntitiesServices;

/// <inheritdoc/>
public class ShiftService : IShiftService
{
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <inheritdoc cref="IAuthService" />
    private readonly IAuthService _authService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ShiftService(
        IDbContextFactory<DataContext> dbFactory, 
        IConfigurationService configurationService, 
        IAuthService authService)
    {
        _dbFactory = dbFactory;
        _configurationService = configurationService;
        _authService = authService;
    }

    /// <inheritdoc/>
    public async Task<Shift?> GetOpenedShiftOrDefaultAsync()
    {
        var shopKey = _configurationService.SettingsFromPosOffice.MainSettings.ShopKey;
        
        await using var db = await _dbFactory.CreateDbContextAsync();

        var groupedShifts = await db.Shifts
            .Where(x => x.ShopKey == shopKey)
            .OrderByDescending(x => x.ShiftKey)
            .GroupBy(s => s.ShiftKey)
            .Select(g => new
            {
                ShiftKey = g.Key,
                OpenRecord = g.FirstOrDefault(s => s.IsOpened!.Value),
                ClosedRecord = g.FirstOrDefault(s => !s.IsOpened!.Value)
            })
            .OrderByDescending(g => g.ShiftKey)
            .FirstOrDefaultAsync();

        if (groupedShifts?.OpenRecord != null && groupedShifts.ClosedRecord == null)
            return groupedShifts.OpenRecord;
        
        return null;
    }

    /// <inheritdoc/>
    public async Task OpenShiftAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var lastShiftNumber = await db.Settings.FindAsync(SettingsKey.Shift);
        var shiftKey = lastShiftNumber != null ? ++lastShiftNumber.Value : 1;
        var shopKey = _configurationService.SettingsFromPosOffice.MainSettings.ShopKey;
        var operatorId = _authService.CurrentUser != null ? _authService.CurrentUser!.UserId : 0;
        
        var newShift = new Shift
        {
            ShiftKey = shiftKey,
            ShopKey = shopKey,
            ShiftDate = DateTime.Now,
            OperatorId = operatorId,
            IsOpened = true
        };

        await db.AddAsync(newShift);

        if (lastShiftNumber != null)
        {
            lastShiftNumber.Value = shiftKey;
            db.Update(lastShiftNumber);
        }

        await db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task CloseShiftAsync(Shift openedShift)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        
        var shopKey = _configurationService.SettingsFromPosOffice.MainSettings.ShopKey;
        var operatorId = _authService.CurrentUser != null ? _authService.CurrentUser!.UserId : 0;
        
        var closingShift = new Shift
        {
            ShiftKey = openedShift.ShiftKey,
            ShopKey = shopKey,
            ShiftDate = DateTime.Now,
            OperatorId = operatorId,
            IsOpened = false
        };
        
        await db.AddAsync(closingShift);
        await db.SaveChangesAsync();
    }
}