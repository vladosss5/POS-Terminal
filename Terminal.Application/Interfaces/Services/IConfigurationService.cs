using Terminal.Core.Models;
using Terminal.Core.Models.Settings;
using Terminal.Core.Models.SettingsFromPosOffice;

namespace Terminal.Application.Interfaces.Services;

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
    /// Сохранить изменения конфигурации в файл.
    /// </summary>
    public void SaveSettingsToFile();

    public List<TableToSendDto> GetTablesToSend();
}