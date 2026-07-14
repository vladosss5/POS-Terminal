using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Core.IRepositories;

public interface IResourceCodeRepository
{
    public Task<List<ResourceCode>> GetShowedResourceCodesAsync();
}