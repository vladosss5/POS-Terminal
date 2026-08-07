namespace Terminal.Core.Enums;

/// <summary>
/// Действия в ходе продажи.
/// </summary>
public enum SaleProcessStep
{
    /// <summary>
    /// Выбор ресурса.
    /// </summary>
    SelectionResourceCode,
    
    /// <summary>
    /// Задание кол-ва единиц ресурса или платёжных средств.
    /// </summary>
    SettingAmount,
    
    /// <summary>
    /// Выбор типа оплаты.
    /// </summary>
    SelectionPaymentType,
    
    /// <summary>
    /// Чтение карты.
    /// </summary>
    CardReading,
    
    /// <summary>
    /// Ввод пин-кода карты.
    /// </summary>
    EnteringPin
}