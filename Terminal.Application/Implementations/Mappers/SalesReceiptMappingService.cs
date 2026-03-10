using Terminal.Application.Interfaces.Mappers;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Mappers;

/// Реализация "<inheritdoc/>"
public class SalesReceiptMappingService : ISalesReceiptMappingService
{
    /// <inheritdoc/>
    public SalesReceipt MapSellingToSalesReceipt(Selling selling)
    {
        return new SalesReceipt
        {
            Number = selling.CheckNumber != null 
                ? selling.CheckNumber.ToString()! 
                : "Номер не определён",
            TerminalNumber = selling.TerminalKey != null 
                ? selling.TerminalKey.ToString()! 
                : "Неизвестный номер",
            CardNumber = selling.IssuerCardId != null 
                ? selling.IssuerCardId.Value.ToString() 
                : "0",
            TransactionDateTime = selling.TransactionDatetime ?? DateTime.MinValue,
            ResourceName = selling.ResourceName ?? "Неизвестный ресурс",
            Amount = selling.Amount ?? 0,
            PricePerUnit = selling.BasePrice ?? 0,
            SellingPrice = selling.BasePrice ?? 0 * selling.Amount ?? 0,
            Discount = (selling.BasePrice ?? 0 * selling.Amount ?? 0) - selling.ClientCost ?? 0,
            TotalPrice = selling.ClientCost ?? 0,
            Operator = selling.PersonKey != null 
                ? selling.PersonKey.ToString()! 
                : "Неизвестный оператор",
            BaseType = selling.BaseType,
            DerivedType = selling.DerivedType
        };
    }
}