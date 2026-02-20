using Terminal.Application.Interfaces.Builders;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Builders;

public class RefuelingProcessBuilder : IRefuelingProcessBuilder
{
    private Refill _refill = new();
    
    public IRefuelingProcessBuilder SetPaymentType(PaymentTypes paymentTypes)
    {
        _refill.PaymentType = paymentTypes;

        return this;
    }

    public IRefuelingProcessBuilder SetFuelType(FuelTypes fuelType)
    {
        _refill.FuelType = fuelType;
        
        return this;
    }

    public IRefuelingProcessBuilder SetAmount(decimal amount)
    {
        _refill.CountFuel = amount;
        _refill.CountMoney = amount;
        return this;
    }

    public Refill Build()
    {
        return _refill;
    }
}