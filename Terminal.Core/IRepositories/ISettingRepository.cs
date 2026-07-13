using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;

namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий настроек.
/// </summary>
public interface ISettingRepository
{
    /// <summary>
    /// Получить настройку по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>Настройка, если не найдена, то null.</returns>
    public Task<Setting?> GetByKeyAsync(SettingsKey key);

    /// <summary>
    /// Обновить настройку по ключу.
    /// </summary>
    /// <param name="setting">Настройка.</param>
    public Task UpdateAsync(Setting setting);
}