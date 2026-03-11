namespace Terminal.Core.Enums;

/// <summary>
/// Дополнительный тип оплаты.
/// </summary>
public enum DerivedPaymentType
{
    /// <summary>
    /// Только наличные.
    /// </summary>
    Cash = 0,
    
    /// <summary>
    /// Топливная карта.
    /// </summary>
    FuelCard = 1,
    
    /// <summary>
    /// Топливная ведомость.
    /// </summary>
    FuelStatement = 2,
    
    /// <summary>
    /// Топливный талон.
    /// </summary>
    FuelTalon = 3,
    
    /// <summary>
    /// Банковская карта.
    /// </summary>
    BankCard = 5,
    
    /// <summary>
    /// При BaseType == Cash + дисконтная/бонусная карта.
    /// </summary>
    Discount = 8
}