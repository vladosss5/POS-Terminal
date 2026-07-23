using System.Text;
using Avalonia.Logging;
using Microsoft.Extensions.Logging;
using Terminal.Application.Dtos.CardInfoRoot;
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

    private readonly IXmlResourceProvider _xmlResourceProvider;
    
    private const int MaxResultBufferSize = 10 * 1024 * 1024;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public DiscountingMethods(
        IDiscountingLibraryService discountingLibrary, 
        IXmlResourceProvider xmlResourceProvider, ILogger<DiscountingMethods> logger)
    {
        _discountingLibrary = discountingLibrary;
        _xmlResourceProvider = xmlResourceProvider;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<string> GetCardInfoAsync(CardInfoRequestDto requestDto)
    {
        try
        {
            // Параллельная загрузка всех необходимых XML-файлов
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

            // Сериализация входных данных
            var inputXml = XmlHelper.SerializeXml(requestDto);

            // Подготовка буфера результата
            var resultBuffer = new byte[MaxResultBufferSize];
            uint returnBytes = 0;

            // Вызов нативной библиотеки
            var resultString = _discountingLibrary.Calculating(
                inputXml,
                limitationXml,
                inputSchema,
                param,
                resultBuffer,
                resultBuffer.Length,
                ref returnBytes);

            // Десериализация результата
            // var response = XmlHelper.DeserializeXml<CardInfoResponseDto>(resultString);

            _logger.LogDebug(
                "Успешно получена информация о карте. Размер ответа: {ResponseSize} байт", 
                returnBytes);

            return resultString;
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
}