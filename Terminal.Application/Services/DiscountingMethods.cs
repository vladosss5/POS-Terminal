using Terminal.Application.Dtos;
using Terminal.Application.Helpers;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class DiscountingMethods : IDiscountingMethods
{
    /// <inheritdoc cref="IDiscountingLibraryService" />
    private readonly IDiscountingLibraryService _discountingLibrary;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DiscountingMethods(IDiscountingLibraryService discountingLibrary)
    {
        _discountingLibrary = discountingLibrary;
    }
    
    /// <inheritdoc/>
    public async Task<CardInfoResponseDto> GetCardInfoAsync(CardInfoRequestDto requestDto)
    {
        var inputXml = XmlHelper.SerializeXml(requestDto);
        var limitationXml = await File.ReadAllTextAsync("limit.xml");
        var inputSchema = await File.ReadAllTextAsync("dsc.xml");
        var result = new byte[10 * 1024 * 1024];
        uint returnBytes = 0;
        
        var resultString = _discountingLibrary.Calculating(
            inputXml, 
            limitationXml, 
            inputSchema, 
            "", 
            result, 
            result.Length, 
            ref returnBytes);

        var resultStr = XmlHelper.DeserializeXml<CardInfoResponseDto>(resultString);

        return resultStr;
    }
}