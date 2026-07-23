using Terminal.Application.Dtos.CardInfoRoot;
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
        var limitationXml = await File.ReadAllTextAsync(Path.Combine("Xmls", "limit.xml"));
        var inputSchema = await File.ReadAllTextAsync(Path.Combine("Xmls", "dsc.xml"));
        var param = await File.ReadAllTextAsync(Path.Combine("Xmls", "param.xml"));
        var result = new byte[10 * 1024 * 1024];
        uint returnBytes = 0;
        
        var resultString = _discountingLibrary.Calculating(
            inputXml, 
            limitationXml, 
            inputSchema, 
            param, 
            result, 
            result.Length, 
            ref returnBytes);

        var response = new CardInfoResponseDto();
        
        try
        {
            response = XmlHelper.DeserializeXml<CardInfoResponseDto>(resultString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return response;
    }
}