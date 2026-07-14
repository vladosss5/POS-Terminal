using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Entities.Models;

namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий смен.
/// </summary>
public interface IShiftRepository
{
    /// <summary>
    /// Добавить запись о смене.
    /// </summary>
    /// <param name="shift">Смена.</param>
    public Task AddAsync(Shift shift);

    /// <summary>
    /// Обновить запись о смене.
    /// </summary>
    /// <param name="shift">Смена.</param>
    public Task UpdateAsync(Shift shift);
    
    /// <summary>
    /// Получить состояние последней смены.
    /// </summary>
    /// <param name="shopKey">Номер торговой точки на которой была работа в последнюю смену.</param>
    /// <returns>Состояние смены.</returns>
    public Task<ShiftStateDto?> GetLastShiftAsync(int shopKey);
}