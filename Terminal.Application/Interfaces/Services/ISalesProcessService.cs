using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Services;

public interface ISalesProcessService
{
    /// <summary>
    /// Получить доступные типы оплат.
    /// </summary>
    /// <returns>Словарь с типами оплаты и их названиями.</returns>
    public Dictionary<string, (BasePaymentType BaseType, DerivedPaymentType DerivedType)> GetAvailablePaymentTypes();

    /// <summary>
    /// Получить доступные для продажи ресурсы.
    /// </summary>
    /// <returns>Список ресурсов.</returns>
    public Task<List<ResourceCode>> GetAvailableResourceCodesAsync();

    /// <summary>
    /// Добавить ресурс в корзину.
    /// </summary>
    /// <param name="resource">Ресурс.</param>
    public void AddToCart(ResourceCode resource);

    /// <summary>
    /// Задать ресурсу в корзине кол-во.
    /// </summary>
    /// <param name="resourceCodeId">Id ресурса.</param>
    /// <param name="amount">Кол-во чего-то.</param>
    /// <param name="calculatedField">Поле требующее вычисление значения.</param>
    public void SetAmount(int resourceCodeId, decimal amount, CalculatedField calculatedField);
    
    /// <summary>
    /// Удалить ресурс из корзины.
    /// </summary>
    /// <param name="resource">Ресурс.</param>
    public void RemoveFromCart(ResourceCode resource);

    /// <summary>
    /// Задать базовый и дополнительный типы оплаты.
    /// </summary>
    /// <param name="baseType">Базовый.</param>
    /// <param name="derivedType">Дополнительный.</param>
    public void SetPaymentType(BasePaymentType baseType, DerivedPaymentType derivedType);

    /// <summary>
    /// Завершить процесс продажи.
    /// </summary>
    public Task CompleteProcessAsync();

    /// <summary>
    /// Прочитать карту.
    /// </summary>
    public Task ReadCardAsync();

    /// <summary>
    /// Ввести PIN карты.
    /// </summary>
    /// <param name="pin">PIN-код.</param>
    public void EnterPin(string pin);
}