using Terminal.Core.Entities.Models;
using Terminal.Core.Entities.Models.Settings;
using Terminal.Core.Entities.Models.SettingsFromPosOffice;

namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис для работы с конфигурацией приложения.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Текущие настройки приложения.
    /// </summary>
    public SettingsModel CurrentSetting { get; set; }
    
    /// <summary>
    /// Настройки приложения из PosOffice. 
    /// </summary>
    public SettingsFromPosOffice SettingsFromPosOffice { get; set; }

    /// <summary>
    /// Обновить настройки из PosOffice.
    /// </summary>
    public Task UpdateSettingsFromPosOffice();

    /// <summary>
    /// Сохранить изменения конфигурации в файл.
    /// </summary>
    public void SaveSettingsToFile();

    /// <summary>
    /// Получить из конфигурации список таблиц из БД для отправки.
    /// </summary>
    /// <returns></returns>
    public List<TableToSendDto> GetTablesToSend();
}