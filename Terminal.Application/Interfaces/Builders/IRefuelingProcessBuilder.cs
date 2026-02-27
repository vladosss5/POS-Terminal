using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Builders;

/// <summary>
/// Строитель процесса заправки.
/// </summary>
public interface IRefuelingProcessBuilder
{
    /// <summary>
    /// Задать тип оплаты.
    /// </summary>
    /// <param name="paymentTypes">Тип оплаты.</param>
    public IRefuelingProcessBuilder SetPaymentType(PaymentTypes paymentTypes);
    
    /// <summary>
    /// Задать тип топлива (товара).
    /// </summary>
    /// <param name="fuelType">Топливо.</param>
    /// <returns></returns>
    public IRefuelingProcessBuilder SetFuelType(ResourceCode fuelType);
    
    /// <summary>
    /// Задать кол-во.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public IRefuelingProcessBuilder SetAmount(decimal amount);
    
    /// <summary>
    /// Собрать итоговый объект процесса заправки.
    /// </summary>
    /// <returns>Модель процесса заправки.</returns>
    public Refill Build();
}