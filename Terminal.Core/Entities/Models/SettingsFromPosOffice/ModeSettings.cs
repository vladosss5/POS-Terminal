using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Terminal.Core.Entities.Models.SettingsFromPosOffice;

/// <summary>
/// Режимы работы.
/// </summary>
public class ModeSettings
{
    /// <summary>
    /// Способ авторизации.
    /// </summary>
    [XmlElement("authorizeMethod")]
    public int AuthorizeMethod { get; set; }
    
    /// <summary>
    /// Очищать при закрытии.
    /// </summary>
    [XmlElement("clearOnClose")]
    public bool ClearOnClose { get; set; }
    
    /// <summary>
    /// Офлайн-режим.
    /// </summary>
    [XmlElement("offline")]
    public bool Offline { get; set; }
    
    /// <summary>
    /// Задержка смены.
    /// </summary>
    [XmlElement("waitShift")]
    public int WaitShift { get; set; }
    
    /// <summary>
    /// Полный режим.
    /// </summary>
    [XmlElement("full")]
    public int Full { get; set; }
    
    /// <summary>
    /// Сессионный режим.
    /// </summary>
    [XmlElement("session")]
    public int Session { get; set; }
    
    /// <summary>
    /// Режим оплаты.
    /// </summary>
    [XmlElement("payment")]
    public bool Payment { get; set; }
    
    /// <summary>
    /// Оплата представителю.
    /// </summary>
    [XmlElement("payToRep")]
    public bool PayToRep { get; set; }
    
    /// <summary>
    /// Игнорировать счет представителя.
    /// </summary>
    [XmlElement("ignoreRepAcc")]
    public bool IgnoreRepAcc { get; set; }
    
    /// <summary>
    /// Восстановление лимита.
    /// </summary>
    [XmlElement("restoreLimit")]
    public bool RestoreLimit { get; set; }
    
    /// <summary>
    /// Разрешить перевод.
    /// </summary>
    [XmlElement("allowTransfer")]
    public bool AllowTransfer { get; set; }
    
    /// <summary>
    /// Возврат платежа.
    /// </summary>
    [XmlElement("repayment")]
    public bool Repayment { get; set; }
    
    /// <summary>
    /// Сенсорный режим.
    /// </summary>
    [XmlElement("touch")]
    public bool Touch { get; set; }
    
    /// <summary>
    /// Режим суммы.
    /// </summary>
    [XmlElement("sum")]
    public bool Sum { get; set; }
    
    /// <summary>
    /// Блокировка счетчика.
    /// </summary>
    [XmlElement("lockCounter")]
    public bool LockCounter { get; set; }
    
    /// <summary>
    /// Бонусы на входе.
    /// </summary>
    [XmlElement("bonusIn")]
    public bool BonusIn { get; set; }
    
    /// <summary>
    /// Пароль на бонусы.
    /// </summary>
    [XmlElement("bonusPassword")]
    public bool BonusPassword { get; set; }
    
    /// <summary>
    /// Частичное списание бонусов.
    /// </summary>
    [XmlElement("partBonus")]
    public bool PartBonus { get; set; }
    
    /// <summary>
    /// Редактирование бонусов.
    /// </summary>
    [XmlElement("editBonus")]
    public bool EditBonus { get; set; }
    
    /// <summary>
    /// Режим начисления бонусов.
    /// </summary>
    [XmlElement("bonusMode")]
    public int BonusMode { get; set; }
    
    /// <summary>
    /// Разрешить LNR.
    /// </summary>
    [XmlElement("allowLNR")]
    public bool AllowLNR { get; set; }
    
    /// <summary>
    /// Нулевой дебет.
    /// </summary>
    [XmlElement("nullDebet")]
    public bool NullDebet { get; set; }
    
    /// <summary>
    /// Лимит объема.
    /// </summary>
    [XmlElement("limitVolume")]
    public int LimitVolume { get; set; }
    
    /// <summary>
    /// Нерабочее время.
    /// </summary>
    [XmlElement("notWorking")]
    public string? NotWorking { get; set; }
}