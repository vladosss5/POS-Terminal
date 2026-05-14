using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Mappers;

/// <summary>
/// Сервис маппинга продаж к чекам.
/// </summary>
public interface ISalesReceiptMappingService
{
    /// <summary>
    /// Преобразовать доменную модель продажи к модели чека о продаже.
    /// </summary>
    /// <param name="selling">Модель продажи.</param>
    /// <returns>Модель чека.</returns>
    public SalesReceipt MapSellingToSalesReceipt(Selling selling);
}