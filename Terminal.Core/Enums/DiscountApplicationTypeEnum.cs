namespace Terminal.Core.Enums;

/// <summary>
/// Типы дисконтных приложений Mifare
/// </summary>
public enum DiscountApplicationTypeEnum
{
    /// <summary>
    /// Тип дисконта для топливных приложений - всегда подменяется на тот, что в схеме
    /// </summary>
    FuelCardDiscountType = 0,
    /// <summary>
    /// Процентный = 1
    /// </summary>
    ProcentDiscountType = 1,
    /// <summary>
    /// Статусный = 2
    /// </summary>
    StatusDiscountType,
    /// <summary>
    /// Накопительный = 3
    /// </summary>
    AccumulationDiscountType,
    /// <summary>
    /// Месячный = 4
    /// </summary>
    AccumulationMonthDiscountType,
    /// <summary>
    /// Особая цена = 5
    /// </summary>
    CostDiscountType,
    /// <summary>
    /// Рублевая скидка = 6
    /// </summary>
    PriceDiscountType,
    /// <summary>
    /// Бонусный = 7
    /// </summary>
    BonusDiscountType,
    /// <summary>
    /// Бонусный месячный = 8
    /// </summary>
    BonusMonthDiscountType,
    /// <summary>
    /// Тип дисконта для лимитных топливных приложений - всегда подменяется на тот, что в схеме
    /// </summary>
    CreditDiscountProgramType = 98,
    /// <summary>
    /// Тип дисконта для дебетных топливных приложений - всегда подменяется на тот, что в схеме
    /// </summary>
    DebitCostDiscountProgramType = 99,
    /// <summary>
    /// Неверный тип
    /// </summary>
    IllegalDiscountType = 100,
    /// <summary>
    /// Партия топлива
    /// </summary>
    DebitConsignmentDiscountProgramType = 101,
    /// <summary>
    /// Наличные
    /// </summary>
    CashDiscountType,
    /// <summary>
    /// Талоны
    /// </summary>
    CouponDiscountType,
    /// <summary>
    /// Ведомости
    /// </summary>
    RegisterDiscountType,
    /// <summary>
    /// Виртуальные карты 
    /// </summary>
    UserCardDiscountType,
    /// <summary>
    /// Партии топлива при покупке их на АЗС при наличных и безналичных расчетах
    /// </summary>
    LNRPartFuelDiscountType
}