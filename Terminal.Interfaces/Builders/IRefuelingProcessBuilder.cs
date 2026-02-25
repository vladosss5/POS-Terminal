using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Interfaces.Builders;

public interface IRefuelingProcessBuilder
{
    public IRefuelingProcessBuilder SetPaymentType(PaymentTypes paymentTypes);
    public IRefuelingProcessBuilder SetFuelType(ResourceCode fuelType);
    public IRefuelingProcessBuilder SetAmount(decimal amount);
    public Refill Build();
}