using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using Terminal.Core.IRepositories;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class ShiftService : IShiftService
{
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <inheritdoc cref="IAuthService" />
    private readonly IAuthService _authService;

    /// <inheritdoc cref="IShiftRepository" />
    private readonly IShiftRepository _shiftRepository;

    /// <inheritdoc cref="ISettingRepository" />
    private readonly ISettingRepository _settingRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ShiftService(
        IConfigurationService configurationService, 
        IAuthService authService, 
        IShiftRepository shiftRepository, 
        ISettingRepository settingRepository)
    {
        _configurationService = configurationService;
        _authService = authService;
        _shiftRepository = shiftRepository;
        _settingRepository = settingRepository;
    }

    /// <inheritdoc/>
    public async Task<Shift?> GetOpenedShiftOrDefaultAsync()
    {
        var shopKey = _configurationService.SettingsFromPosOffice.MainSettings.ShopKey;

        var groupedShifts = await _shiftRepository.GetLastShiftAsync(shopKey);
        
        return groupedShifts is { ClosedRecord: null } 
            ? groupedShifts.OpenRecord 
            : null;
    }

    /// <inheritdoc/>
    public async Task OpenShiftAsync()
    {
        var lastShiftNumber = await _settingRepository.GetByKeyAsync(SettingsKey.Shift);
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

        await _shiftRepository.AddAsync(newShift);

        if (lastShiftNumber != null)
        {
            lastShiftNumber.Value = shiftKey;
            await _settingRepository.UpdateAsync(lastShiftNumber);
        }
    }

    /// <inheritdoc/>
    public async Task CloseShiftAsync(Shift openedShift)
    {
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
        
        await _shiftRepository.AddAsync(closingShift);
    }
}