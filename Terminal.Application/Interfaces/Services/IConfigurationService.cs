using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с конфигурацией приложения.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Получить секцию конфигурации по модели.
    /// </summary>
    /// <param name="sectionName">Наименование секции.</param>
    /// <typeparam name="T">Модель значения секции.</typeparam>
    /// <returns>Секция конфигурации.</returns>
    public T? GetSection<T>(string sectionName) where T : class;
    
    /// <summary>
    /// Получить включенные типы оплат.
    /// </summary>
    /// <returns>Множество моделей настроек типа оплаты.</returns>
    public IEnumerable<PaymentTypeSetting>? GetPaymentTypeSettings();
}