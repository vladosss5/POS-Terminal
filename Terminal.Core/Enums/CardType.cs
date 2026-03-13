namespace Terminal.Core.Enums;

/// <summary>
/// Типы карт, поддерживаемые считывателем Sunyard
/// </summary>
public enum CardType
{
    /// <summary>
    /// MIFARE Classic 1K (S50)
    /// </summary>
    MifareClassic1K = 0,
    
    /// <summary>
    /// MIFARE Classic 4K (S70)
    /// </summary>
    MifareClassic4K = 1,
    
    /// <summary>
    /// PRO Card
    /// </summary>
    MifarePro = 2,
    
    /// <summary>
    /// S50 PRO Card
    /// </summary>
    MifareS50Pro = 3,
    
    /// <summary>
    /// S70 PRO Card
    /// </summary>
    MifareS70Pro = 4,
    
    /// <summary>
    /// CPU Card (EMV)
    /// </summary>
    CpuCard = 5,
    
    /// <summary>
    /// Неизвестный тип
    /// </summary>
    Unknown = 99
}