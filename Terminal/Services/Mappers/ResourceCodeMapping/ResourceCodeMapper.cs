using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Dtos;

namespace Terminal.Services.Mappers.ResourceCodeMapping;

/// <inheritdoc/>
public class ResourceCodeMapper : IResourceCodeMapper
{
    /// <inheritdoc/>
    public ResourceCode MapResourceCodeDtoToDomainModel(ResourceCodeDto dto)
    {
        var resourceCode = new ResourceCode
        {
            FuelCodeKey = dto.FuelCodeKey,
            ResourceKey = dto.ResourceKey,
            ResourceName = dto.ResourceName,
            ResourcePrice = dto.ResourcePrice
        };

        return resourceCode;
    }

    /// <inheritdoc/>
    public ResourceCodeDto MapResourceCodeDomainModelToDto(ResourceCode resourceCode)
    {
        var dto = new ResourceCodeDto
        {
            FuelCodeKey = resourceCode.FuelCodeKey,
            ResourceKey = resourceCode.ResourceKey,
            ResourceName = resourceCode.ResourceName ?? "Без имени",
            ResourcePrice = resourceCode.ResourcePrice ?? 0
        };

        return dto;
    }
}