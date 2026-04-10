using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Views.DialogWindows;

namespace Terminal.Services;

/// <summary>
/// Реализация печати чеков через диалоговое окно.
/// </summary>
public class DialogPrintService
{
    /// <summary>
    /// Ширина страницы.
    /// </summary>
    private const int PageWidth = 48;
    
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<DialogPrintService> _logger;

    private StringBuilder _stringBuilder = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    public DialogPrintService(ILogger<DialogPrintService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Вывести чек о продаже в диалоговом окне.
    /// </summary>
    /// <param name="salesReceipt">Чек о продаже.</param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        try
        {
            _stringBuilder = new StringBuilder();
            
            var receiptText = FormatSalesReceiptText(salesReceipt);

            var result = await ShowTextDialog(receiptText);
            
            return new PrintResult
            {
                Success = result,
                Status = result ? PrinterStatus.Ready : PrinterStatus.Unknown,
                ErrorMessage = result ? null : "Печать отменена пользователем"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при показе диалога печати");
            return new PrintResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Status = PrinterStatus.Unknown
            };
        }
    }

    /// <summary>
    /// Вывести отчёт за смену.
    /// </summary>
    /// <param name="reportData"></param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintShiftReportAsync(ShiftReportDataDto reportData)
    {
        try
        {
            _stringBuilder = new StringBuilder();
            
            var receiptText = FormatShiftReportText(reportData);
            
            var result = await ShowTextDialog(receiptText);
            
            return new PrintResult
            {
                Success = result,
                Status = result ? PrinterStatus.Ready : PrinterStatus.Unknown,
                ErrorMessage = result ? null : "Печать отменена пользователем"
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при показе диалога печати");
            return new PrintResult
            {
                Success = false,
                ErrorMessage = e.Message,
                Status = PrinterStatus.Unknown
            };
        }
    }

    private string FormatShiftReportText(ShiftReportDataDto reportData)
    {
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
        AppendKeyValueLine("Начало:", reportData.Shift.ShiftDate != null ? reportData.Shift.ShiftDate!.Value.ToString(culture) : "");
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
        AppendKeyValueLine("Сумма баз.", totalData.TotalBaseCost.ToString(culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSC.ToString(culture));
        
        AppendTextInCenter("Итого возвратов");
        AppendKeyValueLine("Сумма баз.", totalData.TotalSBCR.ToString(culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSCR.ToString(culture));
        
        AppendTextInCenter("Всего продаж");
        AppendKeyValueLine("Сумма баз.", (totalData.TotalBaseCost - totalData.TotalSBCR).ToString(culture));
        AppendKeyValueLine("Сумма скид.", (totalData.TotalSC - totalData.TotalSCR).ToString(culture));
        
        AppendTextInCenter();
        _stringBuilder.AppendLine($"Оператор: {reportData.OperatorName}");
        AppendLineWidth();
        AppendTextInCenter();
        AppendTextInCenter();
        AppendLineWidth();
        AppendTextInCenter("Подпись");
        
        return _stringBuilder.ToString();
    }

    private void PrintOperationsOnIssuer(string issuerName, IEnumerable<SalesReportResult> operations, bool isPrintTotal)
    {
        var culture = new CultureInfo("ru-RU");
        
        AppendTextInCenter(issuerName);
        
        foreach (var saleData in operations)
        {
            var resourceName = !string.IsNullOrEmpty(saleData.N) ? saleData.N! : "undefined";
            AppendLineWidth(resourceName);
            
            AppendTextInCenter("Продажи");
            AppendKeyValueLine("Ко-во", saleData.A != null ? saleData.A.Value.ToString(culture) : "0");
            AppendKeyValueLine("Сумма баз.", saleData.SBC != null ? saleData.SBC.Value.ToString(culture) : "0");
            AppendKeyValueLine("Сумма скид.", saleData.SC != null ? saleData.SC.Value.ToString(culture) : "0");
            
            AppendTextInCenter("Возвраты");
            AppendKeyValueLine("Ко-во", saleData.AR != null ? saleData.AR.Value.ToString(culture) : "0");
            AppendKeyValueLine("Сумма баз.", saleData.SBCR != null ? saleData.SBCR.Value.ToString(culture) : "0");
            AppendKeyValueLine("Сумма скид.", saleData.SCR != null ? saleData.SCR.Value.ToString(culture) : "0");
            
            AppendTextInCenter($"Итого по {resourceName}");
            AppendKeyValueLine("Ко-во", ((saleData.A ?? 0) - (saleData.AR ?? 0)).ToString(culture));
            AppendKeyValueLine("Сумма баз.", ((saleData.SBC ?? 0) - (saleData.SBCR ?? 0)).ToString(culture));
            AppendKeyValueLine("Сумма скид.", ((saleData.SC ?? 0) - (saleData.SCR ?? 0)).ToString(culture));
        }

        AppendLineWidth();

        if (!isPrintTotal)
            return;
        
        var totalData = new
        {
            TotalSBC = operations.Sum(x => x.SBC ?? 0),
            TotalSC = operations.Sum(x => x.SC ?? 0),
            TotalSBCR = operations.Sum(x => x.SBCR ?? 0),
            TotalSCR = operations.Sum(x => x.SCR ?? 0)
        };
        
        AppendTextInCenter("Итого продаж");
        AppendKeyValueLine("Сумма баз.", totalData.TotalSBC.ToString(culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSC.ToString(culture));
        
        AppendTextInCenter("Итого возвратов");
        AppendKeyValueLine("Сумма баз.", totalData.TotalSBCR.ToString(culture));
        AppendKeyValueLine("Сумма скид.", totalData.TotalSCR.ToString(culture));
        
        AppendTextInCenter("Всего продаж");
        AppendKeyValueLine("Сумма баз.", (totalData.TotalSBC - totalData.TotalSBCR).ToString(culture));
        AppendKeyValueLine("Сумма скид.", (totalData.TotalSC - totalData.TotalSCR).ToString(culture));
        
        AppendLineWidth();
    }

    /// <summary>
    /// Получить форматированный текст чека.
    /// </summary>
    /// <param name="receipt">Чек о покупке.</param>
    /// <returns>Текст чека.</returns>
    private string FormatSalesReceiptText(SalesReceipt receipt)
    {
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
        AppendKeyValueLine(receipt.PricePerUnit.ToString(culture), $"= {receipt.SellingPrice.ToString(culture)}");
        
        AppendKeyValueLine("Скидка", $"= {receipt.Discount.ToString(culture)}");
        AppendKeyValueLine("Итого", $"= {receipt.TotalPrice.ToString(culture)}");
        
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
    private void AppendKeyValueLine(string key, string value)
    {
        var spacer = new string(' ', PageWidth - key.Length - value.Length);
        _stringBuilder.AppendLine(key + spacer + value);
    }

    /// <summary>
    /// Ставить линию растянутую по ширине.
    /// </summary>
    /// <param name="text">Опциональный текст в строке.</param>
    private void AppendLineWidth(string text = "")
    {
        var spacer = new string('-', (PageWidth - text.Length) / 2);
        _stringBuilder.AppendLine(spacer + text + spacer);
    }

    /// <summary>
    /// Добавить текст по центру.
    /// </summary>
    /// <param name="text">Опциональный текст в строке.</param>
    private void AppendTextInCenter(string text = "")
    {
        var spacer = new string(' ', (PageWidth - text.Length) / 2);
        _stringBuilder.AppendLine(spacer + text + spacer);
    }
    
    /// <summary>
    /// Показать диалоговое окно с текстом.
    /// </summary>
    /// <param name="text">Текст для вывода.</param>
    /// <returns>Успешность вывода.</returns>
    private static async Task<bool> ShowTextDialog(string text)
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dialog = new ReceiptPreviewDialogWindow(text);
            return await dialog.ShowModalDialog(desktop.MainWindow!);
        }

        await MessageBoxManager.GetMessageBoxStandard(
                "Чек",
                text,
                ButtonEnum.Ok,
                Icon.None,
                WindowStartupLocation.CenterOwner)
            .ShowAsync();
        
        return true;
    }
}