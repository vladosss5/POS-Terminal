using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Android.Services;

/// <summary>
/// Реализация сервиса печати для Android платформы.
/// </summary>
/// <remarks>
/// НЕ ИСПОЛЬЗОВАТЬ!
/// Сервис выступает заглушкой для работы приложения на Android платформах кроме устройств Sunyard.
/// </remarks>
public class AndroidPrintService : IReceiptPrintService
{
    private readonly ILogger<AndroidPrintService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidPrintService(ILogger<AndroidPrintService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Напечатать чек о продаже.
    /// </summary>
    /// <remarks>
    /// Не реализовано.
    /// </remarks>
    /// <param name="salesReceipt">Объект чека о покупке.</param>
    /// <returns>Результат печати.</returns>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        LogWarning();

        return new PrintResult
        {
            Success = false,
            ErrorMessage = "Принтер не работает на данном устройстве.",
            Status = PrinterStatus.Unknown
        };
    }

    private void LogWarning()
    {
        _logger.LogWarning("Принтер не работает на данном устройстве.");
    }
}