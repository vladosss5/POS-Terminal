using System.Text.Json.Serialization;

namespace Terminal.Core.Models.Settings;

/// <summary>
/// Модель конфигурации терминала.
/// </summary>
public class SettingsModel
{
    /// <summary>
    /// Список типов оплаты.
    /// </summary>
    [JsonPropertyName("PaymentTypes")]
    public List<SettingPaymentType>? PaymentTypes { get; set; }
    
    /// <summary>
    /// Ключ магазина.
    /// </summary>
    [JsonPropertyName("ShopKey")]
    public int ShopKey { get; set; }
    
    /// <summary>
    /// Время ожидания до отмены аутентификации в секундах.
    /// </summary>
    [JsonPropertyName("SecondsAuthenticationCanceled")]
    public short SecondsAuthenticationCanceled { get; set; }
    
    /// <summary>
    /// Информация об организации.
    /// </summary>
    [JsonPropertyName("Organisation")]
    public SettingOrganisation? Organisation { get; set; }
    
    /// <summary>
    /// Очищать при закрытии.
    /// </summary>
    [JsonPropertyName("ClearOnClose")]
    public bool ClearOnClose { get; set; }
    
    /// <summary>
    /// Офлайн-режим.
    /// </summary>
    [JsonPropertyName("Offline")]
    public bool Offline { get; set; }
    
    // Mode
    
    /// <summary>
    /// Задержка смены.
    /// </summary>
    [JsonPropertyName("WaitShift")]
    public int WaitShift { get; set; }
    
    /// <summary>
    /// Полный режим.
    /// </summary>
    [JsonPropertyName("Full")]
    public int Full { get; set; }
    
    /// <summary>
    /// Сессионный режим.
    /// </summary>
    [JsonPropertyName("Session")]
    public int Session { get; set; }
    
    /// <summary>
    /// Режим оплаты.
    /// </summary>
    [JsonPropertyName("Payment")]
    public bool Payment { get; set; }
    
    /// <summary>
    /// Оплата представителю.
    /// </summary>
    [JsonPropertyName("PayToRep")]
    public bool PayToRep { get; set; }
    
    /// <summary>
    /// Игнорировать счет представителя.
    /// </summary>
    [JsonPropertyName("IgnoreRepAcc")]
    public bool IgnoreRepAcc { get; set; }
    
    /// <summary>
    /// Восстановление лимита.
    /// </summary>
    [JsonPropertyName("RestoreLimit")]
    public bool RestoreLimit { get; set; }
    
    /// <summary>
    /// Разрешить перевод.
    /// </summary>
    [JsonPropertyName("AllowTransfer")]
    public bool AllowTransfer { get; set; }
    
    /// <summary>
    /// Возврат платежа.
    /// </summary>
    [JsonPropertyName("Repayment")]
    public bool Repayment { get; set; }
    
    /// <summary>
    /// Сенсорный режим.
    /// </summary>
    [JsonPropertyName("Touch")]
    public bool Touch { get; set; }
    
    /// <summary>
    /// Режим суммы.
    /// </summary>
    [JsonPropertyName("Sum")]
    public bool Sum { get; set; }
    
    /// <summary>
    /// Блокировка счетчика.
    /// </summary>
    [JsonPropertyName("LockCounter")]
    public bool LockCounter { get; set; }
    
    /// <summary>
    /// Бонусы на входе.
    /// </summary>
    [JsonPropertyName("BonusIn")]
    public bool BonusIn { get; set; }
    
    /// <summary>
    /// Пароль на бонусы.
    /// </summary>
    [JsonPropertyName("BonusPassword")]
    public bool BonusPassword { get; set; }
    
    /// <summary>
    /// Частичное списание бонусов.
    /// </summary>
    [JsonPropertyName("PartBonus")]
    public bool PartBonus { get; set; }
    
    /// <summary>
    /// Редактирование бонусов.
    /// </summary>
    [JsonPropertyName("EditBonus")]
    public bool EditBonus { get; set; }
    
    /// <summary>
    /// Режим начисления бонусов.
    /// </summary>
    [JsonPropertyName("BonusMode")]
    public int BonusMode { get; set; }
    
    /// <summary>
    /// Разрешить LNR.
    /// </summary>
    [JsonPropertyName("AllowLNR")]
    public bool AllowLNR { get; set; }
    
    /// <summary>
    /// Нулевой дебет.
    /// </summary>
    [JsonPropertyName("NullDebet")]
    public bool NullDebet { get; set; }
    
    /// <summary>
    /// Лимит объема.
    /// </summary>
    [JsonPropertyName("LimitVolume")]
    public int LimitVolume { get; set; }
    
    /// <summary>
    /// Нерабочее время.
    /// </summary>
    [JsonPropertyName("NotWorking")]
    public string? NotWorking { get; set; }
    
    // Card
    
    /// <summary>
    /// Скидка по карте.
    /// </summary>
    [JsonPropertyName("CardDiscount")]
    public bool CardDiscount { get; set; }
    
    /// <summary>
    /// Топливо по карте.
    /// </summary>
    [JsonPropertyName("CardFuel")]
    public bool CardFuel { get; set; }
    
    /// <summary>
    /// Выписка по карте.
    /// </summary>
    [JsonPropertyName("CardStatement")]
    public bool CardStatement { get; set; }
    
    /// <summary>
    /// Наличные по карте.
    /// </summary>
    [JsonPropertyName("CardCash")]
    public bool CardCash { get; set; }
    
    /// <summary>
    /// Магнитная карта.
    /// </summary>
    [JsonPropertyName("CardMagnetic")]
    public bool CardMagnetic { get; set; }
    
    /// <summary>
    /// Купон по карте.
    /// </summary>
    [JsonPropertyName("CardCoupon")]
    public bool CardCoupon { get; set; }
    
    /// <summary>
    /// Полная карта.
    /// </summary>
    [JsonPropertyName("CardFull")]
    public bool CardFull { get; set; }
    
    /// <summary>
    /// Дебетовая карта.
    /// </summary>
    [JsonPropertyName("CardDebet")]
    public bool CardDebet { get; set; }
    
    /// <summary>
    /// Кредитная карта.
    /// </summary>
    [JsonPropertyName("CardCredit")]
    public bool CardCredit { get; set; }
    
    /// <summary>
    /// Консигнация по карте.
    /// </summary>
    [JsonPropertyName("CardConsignment")]
    public bool CardConsignment { get; set; }
    
    /// <summary>
    /// Ресурс по карте.
    /// </summary>
    [JsonPropertyName("CardResource")]
    public bool CardResource { get; set; }
    
    // Print
    
    /// <summary>
    /// Не печатать.
    /// </summary>
    [JsonPropertyName("DoNotPrint")]
    public bool DoNotPrint { get; set; }
    
    /// <summary>
    /// Показывать цену.
    /// </summary>
    [JsonPropertyName("ShowPrice")]
    public bool ShowPrice { get; set; }
    
    /// <summary>
    /// Бонусная программа.
    /// </summary>
    [JsonPropertyName("BonusProgram")]
    public bool BonusProgram { get; set; }
    
    /// <summary>
    /// Удаленное обновление.
    /// </summary>
    [JsonPropertyName("RemoteUpdate")]
    public bool RemoteUpdate { get; set; }
    
    /// <summary>
    /// Блокировка продажи.
    /// </summary>
    [JsonPropertyName("SaleBlock")]
    public bool SaleBlock { get; set; }
    
    /// <summary>
    /// Тип чека.
    /// </summary>
    [JsonPropertyName("ChequeType")]
    public int ChequeType { get; set; }
    
    /// <summary>
    /// Печать инкассации.
    /// </summary>
    [JsonPropertyName("IncassPrint")]
    public bool IncassPrint { get; set; }
    
    /// <summary>
    /// Копия ресурса.
    /// </summary>
    [JsonPropertyName("ResourceCopy")]
    public bool ResourceCopy { get; set; }
    
    /// <summary>
    /// Копия скидки.
    /// </summary>
    [JsonPropertyName("DiscountCopy")]
    public bool DiscountCopy { get; set; }
    
    /// <summary>
    /// Копия платежа.
    /// </summary>
    [JsonPropertyName("PaymentCopy")]
    public bool PaymentCopy { get; set; }
    
    /// <summary>
    /// Копия выписки.
    /// </summary>
    [JsonPropertyName("StatementCopy")]
    public bool StatementCopy { get; set; }
    
    /// <summary>
    /// Копия купона.
    /// </summary>
    [JsonPropertyName("CouponCopy")]
    public bool CouponCopy { get; set; }
    
    /// <summary>
    /// Количество отчетов.
    /// </summary>
    [JsonPropertyName("ReportCount")]
    public int ReportCount { get; set; }
    
    /// <summary>
    /// Общая сумма.
    /// </summary>
    [JsonPropertyName("TotalAmount")]
    public bool TotalAmount { get; set; }
    
    /// <summary>
    /// Общая скидка.
    /// </summary>
    [JsonPropertyName("TotalDiscount")]
    public bool TotalDiscount { get; set; }
    
    /// <summary>
    /// Тип отчета.
    /// </summary>
    [JsonPropertyName("ReportType")]
    public int ReportType { get; set; }
    
    /// <summary>
    /// Состав отчета.
    /// </summary>
    [JsonPropertyName("ReportCompos")]
    public int ReportCompos { get; set; }
    
    /// <summary>
    /// Элементы отчета.
    /// </summary>
    [JsonPropertyName("ReportItems")]
    public int ReportItems { get; set; }
    
    /// <summary>
    /// Разделение отчета.
    /// </summary>
    [JsonPropertyName("ReportDevide")]
    public bool ReportDevide { get; set; }
    
    /// <summary>
    /// Организация в отчете.
    /// </summary>
    [JsonPropertyName("ReportOrg")]
    public bool ReportOrg { get; set; }
    
    /// <summary>
    /// Итог в отчете.
    /// </summary>
    [JsonPropertyName("ReportTotal")]
    public bool ReportTotal { get; set; }
    
    /// <summary>
    /// Разделение топлива.
    /// </summary>
    [JsonPropertyName("DevideFuels")]
    public bool DevideFuels { get; set; }
    
    // Incass
    
    /// <summary>
    /// Автоматическая инкассация.
    /// </summary>
    [JsonPropertyName("IncassAuto")]
    public bool IncassAuto { get; set; }
    
    /// <summary>
    /// Ожидание инкассации.
    /// </summary>
    [JsonPropertyName("IncassWait")]
    public int IncassWait { get; set; }
    
    /// <summary>
    /// Демонстрационный режим инкассации.
    /// </summary>
    [JsonPropertyName("IncassDemon")]
    public bool IncassDemon { get; set; }
    
    /// <summary>
    /// Таймаут инкассации.
    /// </summary>
    [JsonPropertyName("IncassTimeout")]
    public int IncassTimeout { get; set; }
    
    /// <summary>
    /// Расписание инкассации.
    /// </summary>
    [JsonPropertyName("IncassTimetable")]
    public string? IncassTimetable { get; set; }
    
    // Digits
    
    /// <summary>
    /// Количество знаков объема.
    /// </summary>
    [JsonPropertyName("VolumeCount")]
    public int VolumeCount { get; set; }
    
    /// <summary>
    /// Количество знаков суммы.
    /// </summary>
    [JsonPropertyName("AmountCount")]
    public int AmountCount { get; set; }
    
    /// <summary>
    /// Количество знаков обременения.
    /// </summary>
    [JsonPropertyName("OnusCount")]
    public int OnusCount { get; set; }
    
    /// <summary>
    /// Конфигурация работы.
    /// </summary>
    [JsonPropertyName("WorkConfig")]
    public int WorkConfig { get; set; }
    
    /// <summary>
    /// Пароль сервиса.
    /// </summary>
    [JsonPropertyName("ServicePassword")]
    public string ServicePassword { get; set; } = null!;
    
    /// <summary>
    /// Режим загрузки.
    /// </summary>
    [JsonPropertyName("LoadMode")]
    public int AuthorizeType { get; set; }
    
    /// <summary>
    /// Синхронизация времени.
    /// </summary>
    [JsonPropertyName("SynchroTime")]
    public bool SynchroTime { get; set; }
    
    /// <summary>
    /// Часовой пояс.
    /// </summary>
    [JsonPropertyName("Timezone")]
    public int Timezone { get; set; }
    
    /// <summary>
    /// Режим отладки.
    /// </summary>
    [JsonPropertyName("Debug")]
    public bool Debug { get; set; }
    
    /// <summary>
    /// Простой режим.
    /// </summary>
    [JsonPropertyName("SimpleMode")]
    public bool SimpleMode { get; set; }
    
    /// <summary>
    /// Использовать пин-пад.
    /// </summary>
    [JsonPropertyName("UsePinpad")]
    public bool UsePinpad { get; set; }
    
    /// <summary>
    /// Язык.
    /// </summary>
    [JsonPropertyName("Language")]
    public int Language { get; set; }
}