using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Dtos;

namespace Terminal.Services.Mappers.ResourceCodeMapping;

/// <summary>
/// Маппер ресурсов в дто и наоборот.
/// </summary>
public interface IResourceCodeMapper
{
    /// <summary>
    /// Привести dto к доменной модели ресурса.
    /// </summary>
    /// <param name="dto">Дто ресурса.</param>
    /// <returns>Доменная модель ресурса.</returns>
    public ResourceCode MapResourceCodeDtoToDomainModel(ResourceCodeDto dto);

    /// <summary>
    /// Привести доменную модель ресурса к dto.
    /// </summary>
    /// <param name="resourceCode">Доменная модель ресурса.</param>
    /// <returns>Dto ресурса.</returns>
    public ResourceCodeDto MapResourceCodeDomainModelToDto(ResourceCode resourceCode);
}