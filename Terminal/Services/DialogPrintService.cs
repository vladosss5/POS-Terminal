using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Views.DialogWindows;

namespace Terminal.Services;

/// <summary>
/// Реализация печати чеков через диалоговое окно.
/// </summary>
public class DialogPrintService : IReceiptPrintService
{
    /// <summary>
    /// Ширина страницы.
    /// </summary>
    private const int PageWidth = 48;
    
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<DialogPrintService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    public DialogPrintService(ILogger<DialogPrintService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        try
        {
            var receiptText = FormatReceiptText(salesReceipt);
            
            var dialog = new ReceiptPreviewDialogWindow(receiptText);
            
            bool result;
            
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                result = await dialog.ShowModalDialog(desktop.MainWindow!);
            else
                result = await dialog.ShowModalDialog();
            
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
    /// Получить форматированный текст чека.
    /// </summary>
    /// <param name="receipt">Чек о покупке.</param>
    /// <returns>Текст чека.</returns>
    private string FormatReceiptText(SalesReceipt receipt)
    {
        var culture = new CultureInfo("ru-RU");
        var sb = new StringBuilder();

        AppendKeyValueLine(ref sb, "Чек", $"#{receipt.Number}");
        AppendLineWidth(ref sb);
        AppendKeyValueLine(ref sb, "Терминал:", receipt.TerminalNumber);
        AppendKeyValueLine(ref sb, "Дата:", receipt.TransactionDateTime.ToString(culture));

        if (receipt.BaseType == BasePaymentType.NonCash && receipt.DerivedType == DerivedPaymentType.FuelCard)
        {
            AppendKeyValueLine(ref sb, "Карта", receipt.CardNumber!);
            AppendKeyValueLine(ref sb, "Карта сокр", receipt.CardNumber!);
        }
        AppendLineWidth(ref sb, "Продажа");
        AppendKeyValueLine(ref sb, receipt.ResourceName, $"= {receipt.Amount.ToString(culture)}");
        AppendKeyValueLine(ref sb, receipt.PricePerUnit.ToString(culture), $"= {receipt.SellingPrice.ToString(culture)}");
        
        AppendKeyValueLine(ref sb, "Скидка", $"= {receipt.Discount.ToString(culture)}");
        AppendKeyValueLine(ref sb, "Итого", $"= {receipt.TotalPrice.ToString(culture)}");
        
        if (receipt.BaseType == BasePaymentType.NonCash && receipt.DerivedType == DerivedPaymentType.FuelCard)
        {
            AppendLineWidth(ref sb, "Инфо по кошелькам");
        }

        AppendLineWidth(ref sb);
        sb.AppendLine($"Оператор {receipt.Operator}");
        AppendLineWidth(ref sb);
        
        return sb.ToString();
    }

    /// <summary>
    /// Вставить линию типа ключ-значение.
    /// </summary>
    /// <param name="sb">Изменяемый строитель строки.</param>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    private static void AppendKeyValueLine(ref StringBuilder sb, string key, string value)
    {
        var spacer = new string(' ', PageWidth - key.Length - value.Length);
        sb.AppendLine(key + spacer + value);
    }

    /// <summary>
    /// Ставить линию растянутую по ширине.
    /// </summary>
    /// <param name="sb">Изменяемый строитель строки.</param>
    /// <param name="text">Опциональный текст в строке.</param>
    private static void AppendLineWidth(ref StringBuilder sb, string text = "")
    {
        var spacer = new string('-', (PageWidth - text.Length) / 2);
        sb.AppendLine(spacer + text + spacer);
    }
}