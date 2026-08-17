using MainHelpers.Logger;
using Terminal.Application.Dtos;
using Terminal.Application.Dtos.CardInfoRoot;
using Terminal.Application.Dtos.DebitRoot;
using Terminal.Application.Dtos.DiscountRoot;
using Terminal.Application.Helpers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Exceptions;
using Terminal.Core.Exceptions.ProcessingCenter;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class DiscountingMethods : IDiscountingMethods
{
    /// <inheritdoc cref="ILoggingService" />
    private readonly ILoggingService _logger;
    
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
        ILoggingService logger)
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
        _logger.LogInformation($"Запрошена информация по карте\n{inputXml}");

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

        _logger.LogInformation($"Получена информация по карте\n{resultString}");
        var response = XmlHelper.DeserializeXml<CardInfoDtoResponseDto>(resultString);

        return response;
    }

    /// <inheritdoc/>
    public DiscountResponseDto CalculateDiscount(DiscountRequestDto requestDto)
    {
        var inputXml = XmlHelper.SerializeXml(requestDto);

        var resultBuffer = new byte[MaxResultBufferSize];
        uint returnBytes = 0;
        
        _logger.LogInformation($"Предварительный расчёт скидок. input = {inputXml}");

        var resultString = _discountingLibrary.Calculating(
            inputXml,
            _limitationXml,
            _inputSchema,
            _param,
            resultBuffer,
            resultBuffer.Length,
            ref returnBytes);

        var response = XmlHelper.DeserializeXml<DiscountResponseDto>(resultString);

        _logger.LogInformation($"Успешно рассчитаны скидки. Output - {resultString}");

        return response;
    }

    /// <inheritdoc/>
    public DebitResponseDto Debit(DebitRequestDto requestDto)
    {
        var inputXml = XmlHelper.SerializeXml(requestDto);

        var resultBuffer = new byte[MaxResultBufferSize];
        uint returnBytes = 0;

        _logger.LogInformation($"Начато дебетование. input = {inputXml}");
        var resultString = _discountingLibrary.Calculating(
            inputXml,
            _limitationXml,
            _inputSchema,
            _param,
            resultBuffer,
            resultBuffer.Length,
            ref returnBytes);

        var response = XmlHelper.DeserializeXml<DebitResponseDto>(resultString);
        
        _logger.LogInformation($"Завершено дебетование. Output = {resultString}");

        return response;
    }

    /// <inheritdoc/>
    public void CheckErrorIntoResponse(RequestDto requestDto)
    {
        if (string.IsNullOrEmpty(requestDto.ResultMessageExt))
            return;
        
        var resultMessageExtText = requestDto.ResultMessageExt
            .Replace("&#x0D;", "\r")
            .Replace("&#x0A;", "\n");
        
        var dict = new Dictionary<string, string>();
        var lines = resultMessageExtText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var separatorIndex = line.IndexOf('=');
            
            if (separatorIndex <= 0) 
                continue;
            
            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            dict[key] = value;
        }

        if (requestDto.ResultCodeExt == 65552 || dict["Type"] is "3" or "4")
            throw new RequiredParameterException();

        if (requestDto.ResultCodeExt == 65549 && dict["ViewType"] == "3")
            throw new RequiredPinCodeException();

        if (dict["ViewHeader"].Contains("Недостаточно средств на карте."))
            throw new AmountException();
    }
}