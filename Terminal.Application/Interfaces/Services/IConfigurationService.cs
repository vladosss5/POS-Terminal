namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с конфигурацией приложения.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Загрузить конфигурацию.
    /// </summary>
    /// <remarks>
    /// Вызывается в конструкторе сервиса, но можно вызвать для принудительной загрузки.
    /// </remarks>
    public Task LoadAsync();

    /// <summary>
    /// Получить значение по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T">Тип для десериализации.</typeparam>
    /// <returns></returns>
    public Task<T?> GetValueAsync<T>(string key, T? defaultValue = default);
    
    /// <summary>
    /// Установить значение по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    /// <typeparam name="T">Тип для десериализации.</typeparam>
    /// <returns></returns>
    public Task SetValueAsync<T>(string key, T value);
    
    /// <summary>
    /// Сохранить изменения в файл.
    /// </summary>
    /// <returns></returns>
    public Task SaveAsync();
}