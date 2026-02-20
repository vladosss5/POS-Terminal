using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public class Refill
{
    public PaymentTypes PaymentType { get; set; }
    
    public string FuelType { get; set; }
    
    public decimal CountFuel { get; set; }
    
    public decimal CountMoney { get; set; }
}