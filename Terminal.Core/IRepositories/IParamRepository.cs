using Terminal.Core.Entities.DbEntities.ParamDb;
using Terminal.Core.Enums;

namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий параметров.
/// </summary>
public interface IParamRepository
{
    /// <summary>
    /// Добавить параметр в БД.
    /// </summary>
    /// <param name="param">Параметр.</param>
    public Task AddAsync(Param param);

    /// <summary>
    /// Обновить параметр в БД.
    /// </summary>
    /// <param name="param">Параметр.</param>
    public Task UpdateAsync(Param param);
    
    /// <summary>
    /// Получить параметр из БД по названию.
    /// </summary>
    /// <param name="name">Названия параметров приложения хранящихся в БД.</param>
    /// <returns>Параметр приложения, если не найден, то null.</returns>
    public Task<Param?> GetByNameAsync(AppParameter name);
}