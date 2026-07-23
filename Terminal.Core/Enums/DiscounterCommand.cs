using System.Xml.Serialization;

namespace Terminal.Core.Enums;

/// <summary>
/// Команды для библиотеки расчёта скидок.
/// </summary>
public enum DiscounterCommand
{
    /// <summary>Рассчитать скидку</summary>
    [XmlEnum("0")]
    CalculateDiscount = 0,
    
    /// <summary>Получить информацию по карте</summary>
    [XmlEnum("2")]
    GetCardInfo = 2
}