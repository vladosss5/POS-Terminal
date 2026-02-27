namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с SQL файлами.
/// </summary>
public interface ISqlExecutor
{
    /// <summary>
    /// Выполнить SQL скрипт.
    /// </summary>
    /// <param name="sql">Скрипт.</param>
    /// <returns>Статус код выполнения.</returns>
    public Task<int> ExecuteNonQueryAsync(string sql);
}