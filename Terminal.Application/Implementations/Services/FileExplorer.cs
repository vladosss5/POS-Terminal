using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Data.MainDB;

namespace Terminal.Application.Implementations.Services;

/// <summary>
/// Реализация FileExplorer для не Android платформы.
/// </summary>
public class FileExplorer : IFileExplorer
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<FileExplorer> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public FileExplorer(ILogger<FileExplorer> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task CopyDataBaseDirectoryToDownloadsAsync()
    {
        _logger.LogInformation($"Логика копирования для WIN");
        var sourcePath = DataContext.GetDefaultDbPath();
        var sourceDir = Path.GetDirectoryName(sourcePath);
        
        if (string.IsNullOrEmpty(sourceDir))
            throw new InvalidOperationException("Source directory path is invalid");

        var downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
            "Downloads");
        
        if (!Directory.Exists(downloadsPath))
            downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
        var destDir = Path.Combine(downloadsPath, $"Terminal_DB_Backup_{DateTime.Now:yyyyMMdd_HHmmss}");
        CopyDirectory(sourceDir, destDir);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Копирование директории.
    /// </summary>
    /// <param name="sourceDir">Путь до копируемой директории.</param>
    /// <param name="destDir">Путь до целевой директории.</param>
    private void CopyDirectory(string sourceDir, string destDir)
    {
        try
        {
            Directory.CreateDirectory(destDir);
            
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to copy directory from {sourceDir} to {destDir}", ex);
        }
    }
}