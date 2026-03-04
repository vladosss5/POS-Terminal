using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

/// <summary>
/// Общая реализация сервиса печати.
/// </summary>
/// <remarks>
/// НЕ ИСПОЛЬЗОВАТЬ!
/// Сервис выступает заглушкой для работы приложения на устройствах отличных от Sunyard.
/// </remarks>
public class PrintServiceCommon : IPrintService
{
    /// <inheritdoc/>
    public bool IsConnected { get; }
    
    /// <inheritdoc/>
    /// <remarks>
    /// Не реализовано.
    /// </remarks>
    public async Task<bool> ConnectAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Не реализовано.
    /// </remarks>
    public void Disconnect()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Не реализовано.
    /// </remarks>
    public async Task<PrinterStatus> GetStatusAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Не реализовано.
    /// </remarks>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        throw new NotImplementedException();
    }
}