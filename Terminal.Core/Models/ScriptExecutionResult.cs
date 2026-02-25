namespace Terminal.Core.Models;

public class ScriptExecutionResult
{
    public string FileName { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public int RowsAffected { get; set; }
}