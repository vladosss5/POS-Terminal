using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

public interface IIncassationService
{
    /// <summary>
    /// Собрать данные для инкассации из локальной БД
    /// </summary>
    Task<IncassationData> CollectIncassationDataAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отправить данные инкассации на TMS
    /// </summary>
    Task<IncassationResult> SendIncassationToTmsAsync(CancellationToken cancellationToken = default);
}