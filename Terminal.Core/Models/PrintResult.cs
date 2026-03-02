using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public class PrintResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public PrinterStatus? Status { get; set; }
}