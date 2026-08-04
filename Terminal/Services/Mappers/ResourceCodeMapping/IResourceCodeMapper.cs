using System.Collections.Generic;
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
    /// Привести множество dto к списку доменных моделей ресурса.
    /// </summary>
    /// <param name="dtos">Перечисление dto ресурса.</param>
    /// <returns>Список доменных моделей ресурсов.</returns>
    public List<ResourceCode> MapResourceCodeDtoToDomainModelRange(IEnumerable<ResourceCodeDto> dtos);

    /// <summary>
    /// Привести доменную модель ресурса к dto.
    /// </summary>
    /// <param name="resourceCode">Доменная модель ресурса.</param>
    /// <returns>Dto ресурса.</returns>
    public ResourceCodeDto MapResourceCodeDomainModelToDto(ResourceCode resourceCode);

    /// <summary>
    /// Привести множество доменных моделей ресурса к списку dto.
    /// </summary>
    /// <param name="resourceCodes">Список доменных моделей ресурсов.</param>
    /// <returns>Перечисление dto ресурса.</returns>
    public List<ResourceCodeDto> MapResourceCodeDomainModelToDtoRange(IEnumerable<ResourceCode> resourceCodes);
}