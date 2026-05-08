using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

public interface IEncashmentService
{
    /// <summary>Выполнить инкассацию</summary>
    Task<EncashmentResult> ExecuteEncashmentAsync(CancellationToken cancellationToken = default);
    
    /// <summary>Применить полученные обновления</summary>
    Task<bool> ApplyUpdatesAsync(string updatePath, CancellationToken cancellationToken = default);
    
    /// <summary>Применить полученные таблицы</summary>
    Task<bool> ApplyTablesAsync(IEnumerable<string> tableFiles, CancellationToken cancellationToken = default);
}