using Terminal.Core.DbEntities;

namespace Terminal.Core.Models;

/// <summary>
/// Модель чека о покупке.
/// </summary>
public class SalesReceipt
{
    public string Header { get; set; } = "ООО \"Торговля\"";
    public string Footer { get; set; } = "Спасибо за покупку!";
    public Selling Selling { get; set; } = new();
    public decimal Total { get; set; }
    public bool CutPaper { get; set; } = true;
}