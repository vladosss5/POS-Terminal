namespace Terminal.Application.Interfaces.Background;

/// <summary>
/// Фоновый сервис обновления приложения.
/// </summary>
public interface IUpgradeBackgroundService
{
    /// <summary>
    /// Запустить фоновый процесс обновления приложения.
    /// </summary>
    public Task StartAutoUpgradeAsync();
}