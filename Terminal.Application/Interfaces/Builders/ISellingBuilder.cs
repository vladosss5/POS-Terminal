using Terminal.Core.DbEntities;
using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Builders;

/// <summary>
/// Строитель процесса продажи.
/// </summary>
public interface ISellingBuilder
{
    /// <summary>
    /// Задать базовый и дополнительный типы оплаты.
    /// </summary>
    /// <param name="baseType">Базовый.</param>
    /// <param name="derivedType">Дополнительный.</param>
    public void SetPaymentTypes(BasePaymentType baseType, DerivedPaymentType derivedType);
        
    /// <summary>
    /// Задать атрибутику о ресурсе.
    /// </summary>
    /// <param name="resourceCode">Id ресурса.</param>
    public void SetResourceCode(ResourceCode resourceCode);

    /// <summary>
    /// Задать кол-во.
    /// </summary>
    /// <param name="amount">Кол-во единиц</param>
    public void SetAmount(decimal amount);

    /// <summary>
    /// Задать номер чека.
    /// </summary>
    public Task SetCheckNumber();

    /// <summary>
    /// Задать запрошенный объём топлива/денег.
    /// </summary>
    /// <param name="volume">Запрошенное кол-во.</param>
    /// <param name="isCost">True - кол-во денег.<br/> False - кол-во топлива.</param>
    public void SetRequestedVolume(string volume, bool isCost);

    /// <summary>
    /// Задать оператора.
    /// </summary>
    /// <param name="personKey">Ключ оператора.</param>
    /// <param name="personName">Имя оператора.</param>
    public void SetPersonKey(int personKey, string? personName);

    /// <summary>
    /// Установить номер смены.
    /// </summary>
    public Task SetShiftNumber();

    /// <summary>
    /// Установить номер терминала.
    /// </summary>
    public Task SetTerminalNumber();

    /// <summary>
    /// Установить номер эмитента.
    /// </summary>
    public Task SetIssuerNumber();

    /// <summary>
    /// Собрать итоговый объект процесса заправки.
    /// </summary>
    /// <returns>Модель процесса заправки.</returns>
    public Selling Build();
}