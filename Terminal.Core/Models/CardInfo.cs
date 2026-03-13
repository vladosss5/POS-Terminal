using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Информация о считанной карте (Value Object).
/// </summary>
public sealed record CardInfo
{
    /// <summary>
    /// Уникальный идентификатор карты (UID) в HEX формате.
    /// </summary>
    public string Uid { get; }
    
    /// <summary>
    /// Тип карты.
    /// </summary>
    public CardType Type { get; }
    
    /// <summary>
    /// Сырые данные карты (блок 0).
    /// </summary>
    public byte[] RawData { get; }
    
    /// <summary>
    /// Дополнительная информация (ATR, исторические байты).
    /// </summary>
    public string? AdditionalInfo { get; init; }
    
    /// <summary>
    /// Время считывания.
    /// </summary>
    public DateTime ReadTime { get; }

    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="uid">Уникальный идентификатор карты (UID) в HEX формате.</param>
    /// <param name="type">Тип карты.</param>
    /// <param name="rawData">Сырые данные карты (блок 0).</param>
    public CardInfo(string uid, CardType type, byte[] rawData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uid);
        ArgumentNullException.ThrowIfNull(rawData);
        
        Uid = uid;
        Type = type;
        RawData = rawData;
        ReadTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Форматированный UID для отображения (XX:XX:XX:XX)
    /// </summary>
    public string FormattedUid => string.Join(":", 
        Enumerable.Range(0, Uid.Length / 2)
            .Select(i => Uid.Substring(i * 2, 2)));

    /// <summary>
    /// Проверка, является ли карта платежной (CPU)
    /// </summary>
    public bool IsPaymentCard => Type == CardType.CpuCard;
    
    /// <summary>
    /// Проверка, является ли карта картой доступа (MIFARE)
    /// </summary>
    public bool IsAccessCard => Type is CardType.MifareClassic1K or CardType.MifareClassic4K;
}