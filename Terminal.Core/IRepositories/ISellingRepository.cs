using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий продаж.
/// </summary>
public interface ISellingRepository
{
    /// <summary>
    /// Добавить продажу.
    /// </summary>
    /// <param name="selling">Продажа.</param>
    public Task AddAsync(Selling selling);

    /// <summary>
    /// Обновить продажу.
    /// </summary>
    /// <param name="selling">Продажа.</param>
    public Task UpdateAsync(Selling selling);
}