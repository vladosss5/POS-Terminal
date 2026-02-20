namespace Terminal.Core.Enums;

/// <summary>
/// Типы оплаты.
/// </summary>
public enum PaymentTypes
{
    /// <summary>
    /// Топливная карта.
    /// </summary>
    [FriendlyName("Топливная карта")]
    FuelCard,
    
    /// <summary>
    /// Ведомость.
    /// </summary>
    [FriendlyName("Ведомость")]
    Bucket,
    
    /// <summary>
    /// Наличные.
    /// </summary>
    [FriendlyName("Наличные")]
    Cash,
    
    /// <summary>
    /// Талоны.
    /// </summary>
    [FriendlyName("Талоны")]
    Ticket
}