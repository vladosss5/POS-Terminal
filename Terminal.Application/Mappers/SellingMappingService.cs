using Terminal.Application.Dtos;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;

namespace Terminal.Application.Mappers;

/// Реализация "<inheritdoc/>"
public class SellingMappingService : ISellingMappingService
{
    /// <inheritdoc/>
    public SalesReceipt MapSellingToSalesReceipt(Selling selling)
    {
        var checkNumber = selling.CheckNumber != null ? selling.CheckNumber.ToString()! : "Номер не определён";
        var terminalNumber = selling.TerminalKey != null ? selling.TerminalKey.ToString()! : "Неизвестный номер";
        var cardNumber = selling.IssuerCardId != null ? selling.IssuerCardId.Value.ToString() : "0";
        var currentOperator = !string.IsNullOrEmpty(selling.PersonName) ? selling.PersonName : "Неизвестный оператор";
        
        var sellingPrice = selling.ShopCost ?? 0 / selling.Amount ?? 0;
        var discount = selling.ShopBaseCost ?? 0 - selling.ShopCost ?? 0;
        
        return new SalesReceipt
        {
            Number = checkNumber,
            TerminalNumber = terminalNumber,
            CardNumber = cardNumber,
            TransactionDateTime = selling.TransactionDatetime ?? DateTime.MinValue,
            ResourceName = selling.ResourceName ?? "Неизвестный ресурс",
            Amount = Math.Round(selling.Amount ?? 0, 3),
            PricePerUnit = Math.Round(selling.SellingPrice ?? 0, 2),
            SellingPrice = Math.Round(sellingPrice, 2),
            Discount = Math.Round(discount, 2),
            TotalPrice = Math.Round(selling.ShopCost ?? 0, 2),
            Operator = currentOperator,
            BaseType = selling.BaseType ?? BasePaymentType.Undefined,
            DerivedType = selling.DerivedType
        };
    }

    /// <inheritdoc/>
    public Selling MapSaleInfoDtoToDomainModel(SaleInfoDto saleInfoDto)
    {
        return new Selling
        {
            RequestedCost = saleInfoDto.RequestSum,
            RequestedAmount = saleInfoDto.RequestAmount,
            Amount = saleInfoDto.InitialAmount,
            ShopCost = saleInfoDto.CalculatedSalePrice,
            ShopBaseCost = saleInfoDto.InitialCost,
            BonusInCost = saleInfoDto.InitialBonusIn,
            Density = saleInfoDto.Density,
            RequestFlags = saleInfoDto.Flags,
            Guid = saleInfoDto.TransactionGuid,
            ResourceCode = saleInfoDto.ResourceCode,
            ResourceKey = saleInfoDto.ResourceCode,
            BaseType = (BasePaymentType)saleInfoDto.BasePaymentType,
            DerivedType = (DerivedPaymentType)saleInfoDto.DerivedPaymentType,
            VendorKey = saleInfoDto.VendorCode,
            SellingPrice = saleInfoDto.CalculatedSalePrice * 0.01m,
            TransactionDatetime = saleInfoDto.DateTimeValue,
            ResourceName = saleInfoDto.ResourceName
        };
    }
}