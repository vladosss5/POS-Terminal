using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий пользователей.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получить пользователя по имени.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <returns>Пользователь, если не найден, то null.</returns>
    public Task<User?> GetByUserName(string name);
}