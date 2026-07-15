using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий ресурсов.
/// </summary>
public interface IResourceCodeRepository
{
    /// <summary>
    /// Получить коллекцию всех ресурсов.
    /// </summary>
    /// <returns>Коллекция ресурсов.</returns>
    public Task<List<ResourceCode>> GetResourceCodeCollectionAsync();
    
    /// <summary>
    /// Получить отображаемые ресурсы.
    /// </summary>
    /// <returns>Коллекция ресурсов.</returns>
    public Task<List<ResourceCode>> GetShowedResourceCodesAsync();

    /// <summary>
    /// Получить ресурс по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>Ресурс.</returns>
    public Task<ResourceCode?> GetByResourceKeyAsync(int key);

    public Task UpdateResourceCodeAsync(ResourceCode resourceCode);
}