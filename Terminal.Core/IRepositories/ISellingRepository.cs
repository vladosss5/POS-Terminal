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
    /// Добавить коллекцию продаж.
    /// </summary>
    /// <param name="sales">Коллекция продаж.</param>
    public Task AddRangeAsync(IEnumerable<Selling> sales);

    /// <summary>
    /// Обновить продажу.
    /// </summary>
    /// <param name="selling">Продажа.</param>
    public Task UpdateAsync(Selling selling);

    /// <summary>
    /// Получить запись о продаже по номеру чека.
    /// </summary>
    /// <param name="checkNumber">Номер чека.</param>
    /// <returns>Запись продажи.</returns>
    public Task<Selling?> GetSellingByCheckNumberAsync(int checkNumber);
}