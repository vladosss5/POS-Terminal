using Microsoft.Extensions.Logging;
using Terminal.Application.Dtos.CardInfoRoot;
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
    }

    
    /// <inheritdoc/>
    public async Task<CardInfoDtoResponseDto> GetCardInfoAsync(CardInfoDtoRequestDto dtoRequestDto)
    {
        try
        {
            var loadTasks = new[]
            {
                _xmlResourceProvider.LoadXmlContentAsync("limit.xml"),
                _xmlResourceProvider.LoadXmlContentAsync("dsc.xml"),
                _xmlResourceProvider.LoadXmlContentAsync("param.xml")
            };

            var xmlContents = await Task.WhenAll(loadTasks).ConfigureAwait(false);
            
            var limitationXml = xmlContents[0];
            var inputSchema = xmlContents[1];
            var param = xmlContents[2];
            
            var inputXml = XmlHelper.SerializeXml(dtoRequestDto);

            var resultBuffer = new byte[MaxResultBufferSize];
            uint returnBytes = 0;

            var resultString = _discountingLibrary.Calculating(
                inputXml,
                limitationXml,
                inputSchema,
                param,
                resultBuffer,
                resultBuffer.Length,
                ref returnBytes);

            var response = XmlHelper.DeserializeXml<CardInfoDtoResponseDto>(resultString);

            _logger.LogDebug("Успешно получена информация о карте. Размер ответа: {ResponseSize} байт", returnBytes);

            return response;
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Не удалось найти XML-файлы конфигурации");
            throw new InvalidOperationException(
                "Ошибка инициализации: отсутствуют конфигурационные файлы. Обратитесь к администратору.", 
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении информации о карте");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DiscountResponseDto> CalculateDiscountAsync(DiscountRequestDto requestDto)
    {
        var response = new DiscountResponseDto();
        
        try
        {
            var loadTasks = new[]
            {
                _xmlResourceProvider.LoadXmlContentAsync("limit.xml"),
                _xmlResourceProvider.LoadXmlContentAsync("dsc.xml"),
                _xmlResourceProvider.LoadXmlContentAsync("param.xml")
            };

            var xmlContents = await Task.WhenAll(loadTasks).ConfigureAwait(false);
            
            var limitationXml = xmlContents[0];
            var inputSchema = xmlContents[1];
            var param = xmlContents[2];
        
            var inputXml = XmlHelper.SerializeXml(requestDto);

            var resultBuffer = new byte[MaxResultBufferSize];
            uint returnBytes = 0;

            var resultString = _discountingLibrary.Calculating(
                inputXml,
                limitationXml,
                inputSchema,
                param,
                resultBuffer,
                resultBuffer.Length,
                ref returnBytes);

            response = XmlHelper.DeserializeXml<DiscountResponseDto>(resultString);

            _logger.LogDebug("Успешно рассчитаны скидки. Размер ответа: {ResponseSize} байт", returnBytes);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
        }
        

        return response;
    }
}