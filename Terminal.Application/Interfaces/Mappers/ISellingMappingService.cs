using Terminal.Application.Dtos;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Entities.Models;

namespace Terminal.Application.Interfaces.Mappers;

/// <summary>
/// Сервис маппинга продаж к чекам.
/// </summary>
public interface ISellingMappingService
{
    /// <summary>
    /// Преобразовать доменную модель продажи к модели чека о продаже.
    /// </summary>
    /// <param name="selling">Модель продажи.</param>
    /// <returns>Модель чека.</returns>
    public SalesReceipt MapSellingToSalesReceipt(Selling selling);

    /// <summary>
    /// Преобразовать dto информации о продажи к доменной модели продажи.
    /// </summary>
    /// <param name="saleInfoDto">Dto информации о продаже.</param>
    /// <returns>Доменная модель продажи.</returns>
    public Selling MapSaleInfoDtoToDomainModel(SaleInfoDto saleInfoDto);
}