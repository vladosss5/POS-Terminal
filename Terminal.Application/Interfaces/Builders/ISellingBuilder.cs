using Terminal.Core.DbEntities;
using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Builders;

/// <summary>
/// Строитель процесса продажи.
/// </summary>
public interface ISellingBuilder
{
    /// <summary>
    /// Задать тип оплаты.
    /// </summary>
    /// <param name="type">Тип оплаты.</param>
    public ISellingBuilder SetPaymentType(PaymentTypes type);

    /// <summary>
    /// Задать код продаваемого ресурса.
    /// </summary>
    /// <param name="resourceCode">Id ресурса.</param>
    public ISellingBuilder SetResourceCode(int resourceCode);
    
    /// <summary>
    /// Задать кол-во.
    /// </summary>
    /// <param name="amount">Кол-во единиц</param>
    public ISellingBuilder SetAmount(decimal amount);

    /// <summary>
    /// Собрать итоговый объект процесса заправки.
    /// </summary>
    /// <returns>Модель процесса заправки.</returns>
    public Selling Build();
}