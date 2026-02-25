using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

public class FileReader : IFileReader
{
    public Task<string> ReadAllTextAsync(string path)
        => File.ReadAllTextAsync(path);

    public Task<IEnumerable<string>> GetFilesAsync(string directoryPath, string searchPattern)
    {
        var files = Directory.EnumerateFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
        
        return Task.FromResult(files);
    }

    public bool FileExists(string path) => File.Exists(path);
}