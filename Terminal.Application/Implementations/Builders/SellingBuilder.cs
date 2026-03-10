using Terminal.Application.Interfaces.Builders;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;

namespace Terminal.Application.Implementations.Builders;

/// <inheritdoc/>
public class SellingBuilder : ISellingBuilder
{
    /// <inheritdoc cref="Selling" />
    private readonly Selling _selling = new();

    public void SetPaymentTypes(BasePaymentType baseType, DerivedPaymentType derivedType)
    {
        _selling.BaseType = baseType;
        _selling.DerivedType = derivedType;
    }

    /// <inheritdoc/>
    public void SetResourceCode(ResourceCode resourceCode)
    {
        _selling.ResourceKey = resourceCode.ResourceKey;
        _selling.ResourceCode = resourceCode.ResourceKey;
        _selling.ResourceName = resourceCode.ResourceName;
        _selling.SellingPrice = resourceCode.ResourcePrice;
    }

    /// <inheritdoc/>
    public void SetAmount(decimal amount)
    {
        _selling.Amount = (int)amount;
    }

    /// <inheritdoc/>
    public Selling Build()
    {
        _selling.TransactionDatetime = DateTime.Now;
        _selling.ShopCost = _selling.SellingPrice * _selling.Amount; //TODO: тут расчёт скидок.
        return _selling;
    }
}