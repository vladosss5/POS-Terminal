using System.Xml.Serialization;

namespace Terminal.Core.Enums;

/// <summary>
/// Команды для библиотеки расчёта скидок.
/// </summary>
public enum DiscounterCommand
{
    /// <summary>
    /// Рассчитать скидку.
    /// </summary>
    [XmlEnum("0")]
    CalculateDiscount,
    
    /// <summary>
    /// Получить сообщения по карте.
    /// </summary>
    [XmlEnum("1")]
    Message,
    
    /// <summary>
    /// Получить информацию по карте.
    /// </summary>
    [XmlEnum("2")]
    GetCardInfo,
    
    /// <summary>
    /// Зарегистрировать продажу в онлайне.
    /// </summary>
    [XmlEnum("3")]
    OnlineConfirm,
    
    /// <summary>
    /// Отменить регистрацию продажи в онлайне.
    /// </summary>
    [XmlEnum("4")]
    OnlineReset,
    
    /// <summary>
    /// Подтвердить продажу.
    /// </summary>
    [XmlEnum("5")]
    OnlineComplete,
    
    /// <summary>
    /// Проверить ограничения карты.
    /// </summary>
    [XmlEnum("6")]
    Limitation,
    
    /// <summary>
    /// Начислить сдачу в качестве бонусов.
    /// </summary>
    [XmlEnum("7")]
    ChangeBonus,
    
    /// <summary>
    /// ХЗ.
    /// </summary>
    [XmlEnum("8")]
    PostProcessing,
    
    /// <summary>
    /// Зарегистрировать карту.
    /// </summary>
    [XmlEnum("9")]
    RegistrationCard,
    
    /// <summary>
    /// Проверить продажу в онлайне.
    /// </summary>
    [XmlEnum("10")]
    OnlineCheck,
    
    /// <summary>
    /// Обновить переоценки.
    /// </summary>
    [XmlEnum("11")]
    UpdateMarkdown
}