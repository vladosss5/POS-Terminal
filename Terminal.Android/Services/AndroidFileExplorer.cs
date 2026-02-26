using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Extensions.Logging;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Services;
using Terminal.Data.Context;
using Environment = Android.OS.Environment;

namespace Terminal.Android.Services;

public class AndroidFileExplorer : IFileExplorer
{
    private readonly ILogger<AndroidFileExplorer> _logger;
    
    private readonly Context _context;
    
    private TaskCompletionSource<bool> _permissionTaskCompletionSource;

    public AndroidFileExplorer(
        Context context, 
        ILogger<AndroidFileExplorer> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task CopyDataBaseDirectoryToDownloadsAsync()
    {
        _logger.LogInformation($"Логика копирования для Android");
        
        var sourcePath = DataContext.GetDefaultDbPath();
        var sourceDir = Path.GetDirectoryName(sourcePath);
        
        if (string.IsNullOrEmpty(sourceDir))
            _logger.LogInformation("Source directory path is invalid");

        await CopyToDownloadsScopedStorageAsync(sourceDir);
    }
    
     private async Task CopyToDownloadsScopedStorageAsync(string sourceDir)
    {
        var resolver = _context.ContentResolver;
        var destinationFolder = Path.Combine(Environment.DirectoryDownloads, "Terminal_DB_Backup");
        _logger.LogInformation($"destinationFolder = {destinationFolder}");
        // string destinationFolder = "Terminal_DB_Backup";
        
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

            using var source = File.OpenRead(file);
            using var dest   = resolver.OpenOutputStream(uri);
            _logger.LogInformation($"Копирование");
            await source.CopyToAsync(dest!);
            _logger.LogInformation($"Копирование завершилось");
        }
    }
    
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