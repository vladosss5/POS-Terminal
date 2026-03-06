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

public class DialogPrintService : IReceiptPrintService
{
    private const int PageWidth = 48;
    
    private readonly ILogger<DialogPrintService> _logger;

    public DialogPrintService(ILogger<DialogPrintService> logger)
    {
        _logger = logger;
    }

    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        try
        {
            var receiptText = FormatReceiptText(salesReceipt);
            
            var dialog = new ReceiptPreviewDialogWindow(receiptText);
            
            bool result;
            
            if (Avalonia.Application.Current?.ApplicationLifetime is 
                IClassicDesktopStyleApplicationLifetime desktop)
            {
                result = await dialog.ShowModalDialog(desktop.MainWindow);
            }
            else
            {
                result = await dialog.ShowModalDialog();
            }
            
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

    private string FormatReceiptText(SalesReceipt receipt)
    {
        var culture = new CultureInfo("ru-RU");
        var sb = new StringBuilder();

        AppendKeyValueLine(ref sb, "Чек", $"#{receipt.Number}");
        AppendLineWidth(ref sb);
        AppendKeyValueLine(ref sb, "Терминал:", receipt.TerminalNumber);
        AppendKeyValueLine(ref sb, "Дата:", receipt.TransactionDateTime.ToString(culture));
        
        if (receipt.PaymentTypes == PaymentTypes.FuelCard)
        {
            AppendKeyValueLine(ref sb, "Карта", receipt.CardNumber!);
            AppendKeyValueLine(ref sb, "Карта сокр", receipt.CardNumber!);
        }
        AppendLineWidth(ref sb, "Продажа");
        AppendKeyValueLine(ref sb, receipt.ResourceName, $"= {receipt.Amount.ToString(culture)}");
        AppendKeyValueLine(ref sb, receipt.PricePerUnit.ToString(culture), $"= {receipt.SellingPrice.ToString(culture)}");
        
        AppendKeyValueLine(ref sb, "Скидка", $"= {receipt.Discount.ToString(culture)}");
        AppendKeyValueLine(ref sb, "Итого", $"= {receipt.TotalPrice.ToString(culture)}");
        
        if (receipt.PaymentTypes == PaymentTypes.FuelCard)
        {
            AppendLineWidth(ref sb, "Инфо по кошелькам");
        }

        AppendLineWidth(ref sb);
        sb.AppendLine($"Оператор {receipt.Operator}");
        AppendLineWidth(ref sb);
        
        return sb.ToString();
    }

    private static void AppendKeyValueLine(ref StringBuilder sb, string key, string value)
    {
        var spacer = new string(' ', PageWidth - key.Length - value.Length);
        sb.AppendLine(key + spacer + value);
    }

    private static void AppendLineWidth(ref StringBuilder sb, string text = "")
    {
        var spacer = new string('-', (PageWidth - text.Length) / 2);
        sb.AppendLine(spacer + text + spacer);
    }
}