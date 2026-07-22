using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с параметрами приложения хранящимися в БД.
/// </summary>
public interface IParameterService
{
    /// <summary>
    /// Проверить закончена ли первичная настройка.
    /// </summary>
    /// <returns>True - когда всё настроено.</returns>
    public Task<bool> CheckSetupComplete();
    
    /// <summary>
    /// Получить значение по названию параметра.
    /// </summary>
    /// <param name="parameterName">Название.</param>
    /// <returns>Строковое значение.</returns>
    public Task<string?> GetValueAsync(AppParameter parameterName);

    /// <summary>
    /// Установить значение.
    /// </summary>
    /// <param name="parameterName">Название параметра.</param>
    /// <param name="value">Значение параметра.</param>
    public Task SetValueAsync(AppParameter parameterName, string value);
}