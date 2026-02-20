using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Builders;

public interface IRefuelingProcessBuilder
{
    public IRefuelingProcessBuilder SetPaymentType(PaymentTypes paymentTypes);
    public IRefuelingProcessBuilder SetFuelType(string fuelType);
    public IRefuelingProcessBuilder SetAmount(decimal amount);
    public Refill Build();
}