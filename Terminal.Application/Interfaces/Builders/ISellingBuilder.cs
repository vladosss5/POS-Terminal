using Terminal.Core.DbEntities;
using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Builders;

/// <summary>
/// Строитель процесса продажи.
/// </summary>
public interface ISellingBuilder
{
    /// <summary>
    /// Задать базовый и дополнительный типы оплаты.
    /// </summary>
    /// <param name="baseType">Базовый.</param>
    /// <param name="derivedType">Дополнительный.</param>
    public void SetPaymentTypes(BasePaymentType baseType, DerivedPaymentType derivedType);
        
    /// <summary>
    /// Задать атрибутику о ресурсе.
    /// </summary>
    /// <param name="resourceCode">Id ресурса.</param>
    public void SetResourceCode(ResourceCode resourceCode);

    /// <summary>
    /// Задать кол-во.
    /// </summary>
    /// <param name="amount">Кол-во единиц</param>
    public void SetAmount(decimal amount);

    /// <summary>
    /// Собрать итоговый объект процесса заправки.
    /// </summary>
    /// <returns>Модель процесса заправки.</returns>
    public Selling Build();
}