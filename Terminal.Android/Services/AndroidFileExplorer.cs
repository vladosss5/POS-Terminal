using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Interfaces;
using Terminal.Persistence.MainDB;
using Environment = Android.OS.Environment;

namespace Terminal.Android.Services;

/// <summary>
/// Реализация IFileExplorer под Android платформу.
/// </summary>
public class AndroidFileExplorer : IFileExplorer
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILoggingService _logger;
    
    /// <inheritdoc cref="Context"/>
    private readonly Context _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidFileExplorer(
        Context context, 
        ILoggingService logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task CopyDataBaseDirectoryToDownloadsAsync()
    {
        _logger.LogInformation($"Логика копирования для Android");
        
        var sourcePath = DataContext.GetDefaultDbPath();
        var sourceDir = Path.GetDirectoryName(sourcePath);
        
        if (string.IsNullOrEmpty(sourceDir))
            _logger.LogInformation("Source directory path is invalid");

        var resolver = _context.ContentResolver;
        var destinationFolder = Path.Combine(Environment.DirectoryDownloads, "Terminal_DB_Backup");
        _logger.LogInformation($"destinationFolder = {destinationFolder}");
        
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            _logger.LogInformation($"fileName = {fileName}");
            var contentValues = new ContentValues();
            
            contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            contentValues.Put(MediaStore.IMediaColumns.RelativePath, destinationFolder);
            contentValues.Put(MediaStore.IMediaColumns.MimeType, GetMimeType(fileName));
            
            var uri = resolver.Insert(MediaStore.Files.GetContentUri("external"), contentValues);
            _logger.LogInformation($"uri = {uri}");
            if (uri == null) continue;

            await using var source = File.OpenRead(file);
            await using var dest   = resolver.OpenOutputStream(uri);
            
            _logger.LogInformation($"Копирование");
            await source.CopyToAsync(dest!);
            _logger.LogInformation($"Копирование завершилось");
        }
    }
    
    /// <summary>
    /// Получить тип расширения копируемых объектов.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Тип расширения.</returns>
    private string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".db" => "application/x-sqlite3",
            ".sqlite" => "application/x-sqlite3",
            ".sqlite3" => "application/x-sqlite3",
            _ => "application/octet-stream"
        };
    }
}