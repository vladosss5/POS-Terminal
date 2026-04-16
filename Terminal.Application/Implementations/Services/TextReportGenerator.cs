using System.Globalization;
using System.Text;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

/// <summary>
/// Генератор текста чеков.
/// </summary>
public static class TextReportGenerator
{
    /// <summary>
    /// Ширина страницы.
    /// </summary>
    private const int PageWidth = 48;
    
    /// <summary>
    /// Строитель строки.
    /// </summary>
    private static StringBuilder _stringBuilder = new();
    
    /// <summary>
    /// Получить текст сменного отчёта.
    /// </summary>
    /// <param name="reportData">Данные отчёта.</param>
    /// <returns>Текст отчёта.</returns>
    public static string FormatShiftReportText(ShiftReportDataDto reportData)
    {
        _stringBuilder = new StringBuilder();
        
        var culture = new CultureInfo("ru-RU");

        var title = reportData.ReportType switch
        {
            ShiftReportType.Interim => "Пром. отчёт",
            ShiftReportType.Final => "Итоговый отчёт",
            _ => ""
        };

        AppendLineWidth();
        AppendKeyValueLine("Чек", "#" + reportData.ReceiptNumber);
        AppendLineWidth();

        AppendTextInCenter();
        AppendLineWidth(title);

        AppendKeyValueLine("Эмитент", reportData.IssuerNumber);
        AppendKeyValueLine("Терминал", "#" + reportData.TerminalNumber);
        AppendKeyValueLine("Номер смены:", reportData.Shift.ShiftShopKey.ToString(culture));
        AppendKeyValueLine("Начало:",
            reportData.Shift.ShiftDate != null ? reportData.Shift.ShiftDate!.Value.ToString(culture) : "");
        AppendKeyValueLine("Конец:", DateTime.Now.ToString(culture));

        var issuersCount = reportData.SalesList
            .Select(x => x.ICI)
            .GroupBy(x => x!.Value)
            .Count();

        AppendLineWidth();
        PrintOperationsOnIssuer(
            $"Эмитент {reportData.IssuerNumber}",
            reportData.SalesList.Where(x => x.ICI!.Value == Convert.ToInt32(reportData.IssuerNumber)),
            issuersCount > 1);

        if (issuersCount > 1)
            PrintOperationsOnIssuer(
                $"Другие эмитенты",
                reportData.SalesList.Where(x => x.ICI!.Value != Convert.ToInt32(reportData.IssuerNumber)),
                true);


        var totalData = new
        {
            TotalBaseCost = reportData.SalesList.Sum(x => x.SBC ?? 0),
            TotalSC = reportData.SalesList.Sum(x => x.SC ?? 0),
            TotalSBCR = reportData.SalesList.Sum(x => x.SBCR ?? 0),
            TotalSCR = reportData.SalesList.Sum(x => x.SCR ?? 0)
        };

        AppendLineWidth("Итого в чеке");
        AppendTextInCenter("Итого продаж");
        AppendKeyValueLine("Сумма баз.", totalData.TotalBaseCost.ToString("F2", culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSC.ToString("F2", culture));

        AppendTextInCenter("Итого возвратов");
        AppendKeyValueLine("Сумма баз.", totalData.TotalSBCR.ToString("F2", culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSCR.ToString("F2", culture));

        AppendTextInCenter("Всего продаж");
        AppendKeyValueLine("Сумма баз.", (totalData.TotalBaseCost - totalData.TotalSBCR).ToString("F2", culture));
        AppendKeyValueLine("Сумма скид.", (totalData.TotalSC - totalData.TotalSCR).ToString("F2", culture));

        AppendTextInCenter();
        _stringBuilder.AppendLine($"Оператор: {reportData.OperatorName}");
        AppendLineWidth();
        AppendTextInCenter();
        AppendTextInCenter();
        AppendLineWidth();
        AppendTextInCenter("Подпись");

        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Напечатать продажи по эмитенту.
    /// </summary>
    /// <param name="issuerName">Номер эмитента.</param>
    /// <param name="operations">Коллекция операций.</param>
    /// <param name="isPrintTotal">Печать ли итог чека (не требуется когда эмитент один).</param>
    private static void PrintOperationsOnIssuer(string issuerName, IEnumerable<SalesReportResult> operations, bool isPrintTotal)
    {
        var culture = new CultureInfo("ru-RU");

        AppendTextInCenter(issuerName);

        var salesReportResults = operations as SalesReportResult[] ?? operations.ToArray();
        
        foreach (var saleData in salesReportResults)
        {
            var resourceName = !string.IsNullOrEmpty(saleData.N) ? saleData.N! : "undefined";
            AppendLineWidth(resourceName);

            AppendTextInCenter("Продажи");
            AppendKeyValueLine("Ко-во", saleData.A != null ? saleData.A.Value.ToString("F3", culture) : "0,000");
            AppendKeyValueLine("Сумма баз.", saleData.SBC != null ? saleData.SBC.Value.ToString("F2", culture) : "0,00");
            AppendKeyValueLine("Сумма скид.", saleData.SC != null ? saleData.SC.Value.ToString("F2", culture) : "0,00");

            AppendTextInCenter("Возвраты");
            AppendKeyValueLine("Ко-во", saleData.AR != null ? saleData.AR.Value.ToString("F3", culture) : "0,000");
            AppendKeyValueLine("Сумма баз.", saleData.SBCR != null ? saleData.SBCR.Value.ToString("F2", culture) : "0,00");
            AppendKeyValueLine("Сумма скид.", saleData.SCR != null ? saleData.SCR.Value.ToString("F2", culture) : "0,00");

            AppendTextInCenter($"Итого по {resourceName}");
            AppendKeyValueLine("Ко-во", ((saleData.A ?? 0) - (saleData.AR ?? 0)).ToString("F3", culture));
            AppendKeyValueLine("Сумма баз.", ((saleData.SBC ?? 0) - (saleData.SBCR ?? 0)).ToString("F2", culture));
            AppendKeyValueLine("Сумма скид.", ((saleData.SC ?? 0) - (saleData.SCR ?? 0)).ToString("F2", culture));
        }

        AppendLineWidth();

        if (!isPrintTotal)
            return;

        var totalData = new
        {
            TotalSBC = salesReportResults.Sum(x => x.SBC ?? 0),
            TotalSC = salesReportResults.Sum(x => x.SC ?? 0),
            TotalSBCR = salesReportResults.Sum(x => x.SBCR ?? 0),
            TotalSCR = salesReportResults.Sum(x => x.SCR ?? 0)
        };

        AppendTextInCenter("Итого продаж");
        AppendKeyValueLine("Сумма баз.", totalData.TotalSBC.ToString("F2", culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSC.ToString("F2", culture));

        AppendTextInCenter("Итого возвратов");
        AppendKeyValueLine("Сумма баз.", totalData.TotalSBCR.ToString("F2", culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSCR.ToString("F2", culture));

        AppendTextInCenter("Всего продаж");
        AppendKeyValueLine("Сумма баз.", (totalData.TotalSBC - totalData.TotalSBCR).ToString("F2", culture));
        AppendKeyValueLine("Сумма скид.", (totalData.TotalSC - totalData.TotalSCR).ToString("F2", culture));

        AppendLineWidth();
    }
    
    /// <summary>
    /// Получить форматированный текст чека.
    /// </summary>
    /// <param name="receipt">Чек о покупке.</param>
    /// <returns>Текст чека.</returns>
    public static string FormatSalesReceiptText(SalesReceipt receipt)
    {
        _stringBuilder = new StringBuilder();
        
        var culture = new CultureInfo("ru-RU");

        AppendKeyValueLine("Чек", $"#{receipt.Number}");
        AppendLineWidth();
        AppendKeyValueLine("Терминал:", receipt.TerminalNumber);
        AppendKeyValueLine("Дата:", receipt.TransactionDateTime.ToString(culture));

        if (receipt.BaseType == BasePaymentType.NonCash && receipt.DerivedType == DerivedPaymentType.FuelCard)
        {
            AppendKeyValueLine("Карта", receipt.CardNumber!);
            AppendKeyValueLine("Карта сокр", receipt.CardNumber!);
        }
        AppendLineWidth("Продажа");
        AppendKeyValueLine(receipt.ResourceName, $"= {receipt.Amount.ToString(culture)}");
        AppendKeyValueLine(receipt.PricePerUnit.ToString("F2", culture), $"= {receipt.SellingPrice.ToString("F2", culture)}");
        
        AppendKeyValueLine("Скидка", $"= {receipt.Discount.ToString("F2", culture)}");
        AppendKeyValueLine("Итого", $"= {receipt.TotalPrice.ToString("F2", culture)}");
        
        if (receipt.BaseType == BasePaymentType.NonCash && receipt.DerivedType == DerivedPaymentType.FuelCard)
        {
            AppendLineWidth("Инфо по кошелькам");
        }

        AppendLineWidth();
        _stringBuilder.AppendLine($"Оператор {receipt.Operator}");
        AppendLineWidth();
        
        return _stringBuilder.ToString();
    }
    
    /// <summary>
    /// Вставить линию типа ключ-значение.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    private static void AppendKeyValueLine(string key, string value)
    {
        var spacer = new string(' ', PageWidth - key.Length - value.Length);
        _stringBuilder.AppendLine(key + spacer + value);
    }

    /// <summary>
    /// Ставить линию растянутую по ширине.
    /// </summary>
    /// <param name="text">Опциональный текст в строке.</param>
    private static void AppendLineWidth(string text = "")
    {
        var spacer = new string('-', (PageWidth - text.Length) / 2);
        _stringBuilder.AppendLine(spacer + text + spacer);
    }

    /// <summary>
    /// Добавить текст по центру.
    /// </summary>
    /// <param name="text">Опциональный текст в строке.</param>
    private static void AppendTextInCenter(string text = "")
    {
        var spacer = new string(' ', (PageWidth - text.Length) / 2);
        _stringBuilder.AppendLine(spacer + text + spacer);
    }
}