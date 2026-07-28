using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Microsoft.Extensions.Logging;
using Terminal.Core.Interfaces;

namespace Terminal.Android.Services;

/// <summary>
/// Android-реализация провайдера XML-ресурсов через AssetManager.
/// </summary>
public sealed class AndroidXmlResourceProvider : IXmlResourceProvider
{
    /// <inheritdoc cref="Context" />
    private readonly Context _context;

    /// <inheritdoc cref="ILogger" />
    private readonly ILogger<AndroidXmlResourceProvider> _logger;
    
    /// <summary>
    /// Название каталога в котором хранятся xml файлы.
    /// </summary>
    private const string XmlAssetFolder = "Xmls";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidXmlResourceProvider(
        Context context, 
        ILogger<AndroidXmlResourceProvider> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> LoadXmlContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var assetPath = $"{XmlAssetFolder}/{fileName}";

        try
        {
            await using var stream = _context.Assets!.Open(assetPath);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync(cancellationToken);
        }
        catch (Java.IO.FileNotFoundException ex)
        {
            _logger.LogError($"ERROR! {ex.Message}");
            throw new FileNotFoundException($"XML-файл не найден в Assets: {assetPath}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR! {ex.Message}");
            return "";
        }
    }

    /// <inheritdoc />
    public async Task<bool> XmlFileExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var assets = await _context.Assets!.ListAsync(XmlAssetFolder);
            return assets.Contains(fileName);
        }
        catch
        {
            return false;
        }
    }
}