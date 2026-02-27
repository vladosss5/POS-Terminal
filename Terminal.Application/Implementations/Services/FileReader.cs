using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc cref="IFileReader"/>
public class FileReader : IFileReader
{
    /// <inheritdoc/>
    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

    /// <inheritdoc/>
    public Task<IEnumerable<string>> GetFilesAsync(string directoryPath, string searchPattern)
    {
        var files = Directory.EnumerateFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
        
        return Task.FromResult(files);
    }

    /// <inheritdoc/>
    public bool FileExists(string path) => File.Exists(path);
}