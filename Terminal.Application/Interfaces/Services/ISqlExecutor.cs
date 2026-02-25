namespace Terminal.Application.Interfaces.Services;

public interface ISqlExecutor
{
    public Task<int> ExecuteNonQueryAsync(string sql);
}