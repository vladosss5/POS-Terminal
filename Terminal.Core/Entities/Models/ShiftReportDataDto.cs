using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// ДТО с данными для печати сменного отчёта.
/// </summary>
public class ShiftReportDataDto
{
    /// <summary>
    /// Номер чека.
    /// </summary>
    public int ReceiptNumber { get; set; }
    
    /// <summary>
    /// Номер эмитента.
    /// </summary>
    public string IssuerNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Номер терминала.
    /// </summary>
    public string TerminalNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Смена за которую составляется отчёт.
    /// </summary>
    public Shift Shift { get; set; } = null!;
    
    /// <summary>
    /// Продажи в указанную смену.
    /// </summary>
    public List<SalesReportResult> SalesList { get; set; } = [];
    
    /// <summary>
    /// Тип отчёта.
    /// </summary>
    public ShiftReportType ReportType { get; set; }
    
    /// <summary>
    /// Имя оператора.
    /// </summary>
    public string? OperatorName { get; set; }
}