using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Core.Interfaces;

namespace Terminal.Desktop.Services;

public class DesktopXmlResourceProvider : IXmlResourceProvider
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<DesktopXmlResourceProvider> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public DesktopXmlResourceProvider(ILogger<DesktopXmlResourceProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> LoadXmlContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Xmls", fileName);

        if (!File.Exists(path))
        {
            _logger.LogError($"Файл {fileName} не найден.");
            throw new Exception($"Файл {fileName} не найден.");
        }

        var resultText = await File.ReadAllTextAsync(path, cancellationToken);

        return resultText;
    }

    /// <inheritdoc/>
    public async Task<bool> XmlFileExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}