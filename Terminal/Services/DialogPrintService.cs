using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <param name="salesReportData"></param>
    /// <param name="shift"></param>
    /// <param name="reportType">Тип отчёта: промежуточный или итоговый.</param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintShiftReportAsync(List<SalesReportResult> salesReportData,
        Shift shift, ShiftReportType reportType)
    {
        try
        {
            _stringBuilder = new StringBuilder();
            
            var receiptText = FormatShiftReportText(salesReportData, shift, reportType);
            
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

    private string FormatShiftReportText(List<SalesReportResult> salesReportData, Shift shift, ShiftReportType reportType)
    {
        var culture = new CultureInfo("ru-RU");
        
        var title = reportType switch
        {
            ShiftReportType.Interim => "Пром. отчёт",
            ShiftReportType.Final => "Итоговый отчёт",
            _ => ""
        };

        AppendLineWidth(title);
        
        AppendKeyValueLine("Номер смены:", shift.ShiftShopKey.ToString(culture));
        AppendKeyValueLine("Начало:", shift.ShiftDate != null ? shift.ShiftDate!.Value.ToString(culture) : "");
        AppendKeyValueLine("Конец:", DateTime.Now.ToString(culture));

        foreach (var saleData in salesReportData)
        {
            AppendLineWidth(saleData.N);
            
            AppendTextInCenter("Продажи");
            AppendKeyValueLine("Ко-во", saleData.A.ToString(culture));
            AppendKeyValueLine("Сумма баз.", saleData.SBC.ToString(culture));
            AppendKeyValueLine("Сумма скид.", saleData.SC.ToString(culture));
            
            AppendTextInCenter("Возвраты");
            AppendKeyValueLine("Ко-во", saleData.AR.ToString(culture));
            AppendKeyValueLine("Сумма баз.", saleData.SBCR.ToString(culture));
            AppendKeyValueLine("Сумма скид.", saleData.SCR.ToString(culture));
        }
        
        return _stringBuilder.ToString();
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

    private void AppendTextInCenter(string text)
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