namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис инкассации.
/// </summary>
public interface IEncashmentService
{
    /// <summary>
    /// Выполнить инкассацию.
    /// </summary>
    public Task EncashmentAsync();
}