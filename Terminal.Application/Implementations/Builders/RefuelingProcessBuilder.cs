using Terminal.Application.Interfaces.Builders;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Builders;

/// <inheritdoc cref="IRefuelingProcessBuilder" />
public class RefuelingProcessBuilder : IRefuelingProcessBuilder
{
    /// <summary>
    /// Процесс заправки.
    /// </summary>
    private Refill _refill = new();
    
    /// <inheritdoc/>
    public IRefuelingProcessBuilder SetPaymentType(PaymentTypes paymentTypes)
    {
        _refill.PaymentType = paymentTypes;

        return this;
    }

    /// <inheritdoc/>
    public IRefuelingProcessBuilder SetFuelType(ResourceCode fuelType)
    {
        _refill.FuelType = fuelType;
        
        return this;
    }

    /// <inheritdoc/>
    public IRefuelingProcessBuilder SetAmount(decimal amount)
    {
        _refill.CountFuel = amount;
        _refill.CountMoney = amount;
        return this;
    }

    /// <inheritdoc/>
    public Refill Build()
    {
        return _refill;
    }
}