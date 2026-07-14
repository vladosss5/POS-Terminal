namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис обновления конфигурации.
/// </summary>
public interface IConfigurationUpdatingService
{
    /// <summary>
    /// Обновить конфигурацию из TMS.
    /// </summary>
    public Task UpdateSettingsFromPosTms();
}