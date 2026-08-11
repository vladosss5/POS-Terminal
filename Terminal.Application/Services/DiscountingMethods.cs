using Microsoft.Extensions.Logging;
using Terminal.Application.Dtos.CardInfoRoot;
using Terminal.Application.Dtos.DebitRoot;
using Terminal.Application.Dtos.DiscountRoot;
using Terminal.Application.Helpers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class DiscountingMethods : IDiscountingMethods
{
    private readonly ILogger<DiscountingMethods> _logger;
    
    /// <inheritdoc cref="IDiscountingLibraryService" />
    private readonly IDiscountingLibraryService _discountingLibrary;

    /// <inheritdoc cref="IXmlResourceProvider" />
    private readonly IXmlResourceProvider _xmlResourceProvider;
    
    /// <summary>
    /// Максимальный размер буфера.
    /// </summary>
    private const int MaxResultBufferSize = 10 * 1024 * 1024;

    private string _limitationXml = "";
    private string _inputSchema = "";
    private string _param = "";
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DiscountingMethods(
        IDiscountingLibraryService discountingLibrary, 
        IXmlResourceProvider xmlResourceProvider, 
        ILogger<DiscountingMethods> logger)
    {
        _discountingLibrary = discountingLibrary;
        _xmlResourceProvider = xmlResourceProvider;
        _logger = logger;

        _ = InitXmlsAsync();
    }


    private async Task InitXmlsAsync()
    {
        var loadTasks = new[]
        {
            _xmlResourceProvider.LoadXmlContentAsync("limit.xml"),
            _xmlResourceProvider.LoadXmlContentAsync("dsc.xml"),
            _xmlResourceProvider.LoadXmlContentAsync("param.xml")
        };

        var xmlContents = await Task.WhenAll(loadTasks).ConfigureAwait(false);
            
        _limitationXml = xmlContents[0];
        _inputSchema = xmlContents[1];
        _param = xmlContents[2];
    }
    
    /// <inheritdoc/>
    public CardInfoDtoResponseDto GetCardInfo(CardInfoDtoRequestDto dtoRequestDto)
    {
        var inputXml = XmlHelper.SerializeXml(dtoRequestDto);

        var resultBuffer = new byte[MaxResultBufferSize];
        uint returnBytes = 0;

        var resultString = _discountingLibrary.Calculating(
            inputXml,
            _limitationXml,
            _inputSchema,
            _param,
            resultBuffer,
            resultBuffer.Length,
            ref returnBytes);

        var response = XmlHelper.DeserializeXml<CardInfoDtoResponseDto>(resultString);

        _logger.LogDebug("Успешно получена информация о карте. Размер ответа: {ResponseSize} байт", returnBytes);

        return response;
    }

    /// <inheritdoc/>
    public DiscountResponseDto CalculateDiscount(DiscountRequestDto requestDto)
    {
        var inputXml = XmlHelper.SerializeXml(requestDto);

        var resultBuffer = new byte[MaxResultBufferSize];
        uint returnBytes = 0;
        
        _logger.LogDebug("Предварительный расчёт скидок. input = {inputXml}", inputXml);

        var resultString = _discountingLibrary.Calculating(
            inputXml,
            _limitationXml,
            _inputSchema,
            _param,
            resultBuffer,
            resultBuffer.Length,
            ref returnBytes);

        var response = XmlHelper.DeserializeXml<DiscountResponseDto>(resultString);

        _logger.LogDebug("Успешно рассчитаны скидки. Output - {resultString}", resultString);

        return response;
    }

    public DebitResponseDto Debit(DebitRequestDto requestDto)
    {
        var inputXml = XmlHelper.SerializeXml(requestDto);

        var resultBuffer = new byte[MaxResultBufferSize];
        uint returnBytes = 0;

        _logger.LogDebug("Начато дебетование. input = {inputXml}", inputXml);
        var resultString = _discountingLibrary.Calculating(
            inputXml,
            _limitationXml,
            _inputSchema,
            _param,
            resultBuffer,
            resultBuffer.Length,
            ref returnBytes);

        var response = XmlHelper.DeserializeXml<DebitResponseDto>(resultString);
        
        _logger.LogDebug("Завершено дебетование. Output = {resultString}", resultString);

        return response;
    }
}