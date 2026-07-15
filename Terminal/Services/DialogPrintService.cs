using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Terminal.Application.Services;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Views.DialogWindows;

namespace Terminal.Services;

/// <summary>
/// Реализация печати чеков через диалоговое окно.
/// </summary>
public class DialogPrintService
{
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

    /// <summary>
    /// Вывести чек о продаже в диалоговом окне.
    /// </summary>
    /// <param name="salesReceipt">Чек о продаже.</param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        try
        {
            var receiptText = TextReportGenerator.FormatSalesReceiptText(salesReceipt);

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
    /// <param name="reportData">Данные для печати чека.</param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintShiftReportAsync(ShiftReportDataDto reportData)
    {
        try
        {
            var receiptText = TextReportGenerator.FormatShiftReportText(reportData);
            
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

    /// <summary>
    /// Печать чека об изменение цены ресурса.
    /// </summary>
    /// <param name="changeData">Данные для печати.</param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintPriceChangeAsync(PriceChangeData changeData)
    {
        try
        {
            var receiptText = TextReportGenerator.FormatPriceChangeText(changeData);
            
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