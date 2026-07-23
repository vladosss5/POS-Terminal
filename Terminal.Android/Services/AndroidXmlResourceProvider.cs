// Terminal.Android/Services/AndroidXmlResourceProvider.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Terminal.Core.Interfaces;

namespace Terminal.Android.Services;

/// <summary>
/// Android-реализация провайдера XML-ресурсов через AssetManager
/// </summary>
public sealed class AndroidXmlResourceProvider : IXmlResourceProvider
{
    private readonly Context _context;
    private const string XmlAssetFolder = "Xmls";

    /// <summary>
    /// Инициализирует провайдер с Android-контекстом
    /// </summary>
    /// <param name="context">Android-контекст приложения</param>
    public AndroidXmlResourceProvider(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<string> LoadXmlContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var assetPath = $"{XmlAssetFolder}/{fileName}";
        
        try
        {
            using var stream = _context.Assets!.Open(assetPath);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync(cancellationToken);
        }
        catch (Java.IO.FileNotFoundException ex)
        {
            throw new FileNotFoundException($"XML-файл не найден в Assets: {assetPath}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> XmlFileExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var assetPath = $"{XmlAssetFolder}/{fileName}";
        
        try
        {
            var assets = _context.Assets!.List(XmlAssetFolder)
                .ToArray();
            return assets.Contains(fileName);
        }
        catch
        {
            return false;
        }
    }
}