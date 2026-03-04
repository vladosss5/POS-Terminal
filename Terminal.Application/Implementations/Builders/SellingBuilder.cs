using Terminal.Application.Interfaces.Builders;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;

namespace Terminal.Application.Implementations.Builders;

/// <inheritdoc/>
public class SellingBuilder : ISellingBuilder
{
    /// <inheritdoc cref="Selling" />
    private readonly Selling _selling = new();
    
    /// <inheritdoc/>
    public ISellingBuilder SetPaymentType(PaymentTypes type)
    {
        _selling.PaymentType = type;

        return this;
    }

    /// <inheritdoc/>
    public ISellingBuilder SetResourceCode(int resourceCode)
    {
        _selling.ResourceCode = resourceCode;
        return this;
    }

    /// <inheritdoc/>
    public ISellingBuilder SetAmount(decimal amount)
    {
        _selling.Amount = (int)amount;
        return this;
    }

    /// <inheritdoc/>
    public Selling Build()
    {
        return _selling;
    }
}