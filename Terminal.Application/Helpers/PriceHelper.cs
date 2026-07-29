namespace Terminal.Application.Helpers;

/// <summary>
/// Помощник расчёта цен и количества.
/// </summary>
public static class PriceHelper
{
    /// <summary>
    /// Рассчитать количество из цены за единицу и запрашиваемой цены. 
    /// </summary>
    /// <param name="resourcePrice">Цена за единицу.</param>
    /// <param name="requestSum">Запрашиваемая цена.</param>
    /// <param name="round">Кол-во цифр после запятой. 3 - по-умолчанию.</param>
    /// <returns>Количество ресурса.</returns>
    public static decimal CalculateAmount(decimal resourcePrice, decimal requestSum, int round = 3)
    {
        var amount = requestSum / resourcePrice;
        return Math.Round(amount, round);
    }

    /// <summary>
    /// Рассчитать цену из цены за единицу и запрашиваемого кол-ва. 
    /// </summary>
    /// <param name="resourcePrice">Цена за единицу.</param>
    /// <param name="requestAmount">Запрашиваемое кол-во.</param>
    /// <param name="round">Кол-во цифр после запятой. 2 - по-умолчанию.</param>
    /// <returns>Цена.</returns>
    public static decimal CalculatePrice(decimal resourcePrice, decimal requestAmount, int round = 2)
    {
        var price = resourcePrice * requestAmount;
        return Math.Round(price, round);
    }
}