using Terminal.Core.DbEntities;
using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Тестовая модель процесса заправки.
/// </summary>
public class Refill // TODO: Удалить эту модель. Переделать логику на модель БД.
{
    public PaymentTypes PaymentType { get; set; }
    
    public ResourceCode FuelType { get; set; }
    
    public decimal CountFuel { get; set; }
    
    public decimal CountMoney { get; set; }
}