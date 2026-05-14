using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Application.Interfaces.DbEntitiesServices;

/// <summary>
/// Сервис для работы со сменами.
/// </summary>
public interface IShiftService
{
    /// <summary>
    /// Получить открытую смену или значение по-умолчанию.
    /// </summary>
    /// <returns>Последняя открытуя смена или значение по-умолчанию.</returns>
    public Task<Shift?> GetOpenedShiftOrDefaultAsync();
    
    /// <summary>
    /// Открыть смену.
    /// </summary>
    /// <returns>Открытая смена.</returns>
    public Task OpenShiftAsync();
    
    /// <summary>
    /// Закрыть смену.
    /// </summary>
    /// <param name="openedShift">Ранее открытая смена.</param>
    /// <returns>Закрытая смена.</returns>
    public Task CloseShiftAsync(Shift openedShift);
}