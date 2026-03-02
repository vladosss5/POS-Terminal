using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

public interface IPrintService
{
    public bool IsConnected { get; }
    public event EventHandler<bool> ConnectionChanged;
    public event EventHandler<string> ErrorOccurred;

    public Task<bool> ConnectAsync();
    public void Disconnect();
    public Task<PrinterStatus> GetStatusAsync();
    public Task<PrintResult> PrintReceiptAsync(Receipt receipt);
}