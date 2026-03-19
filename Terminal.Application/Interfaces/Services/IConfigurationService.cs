namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с конфигурацией приложения.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Получить значение по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T">Тип для десериализации.</typeparam>
    /// <returns>Десериализованный объект настройки.</returns>
    public Task<T?> GetValueAsync<T>(string key, T? defaultValue = default);
    
    /// <summary>
    /// Установить значение по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    /// <typeparam name="T">Тип для десериализации.</typeparam>
    public Task SetValueAsync<T>(string key, T value);
}