using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.Application.UseCases;

public class ExecuteSqlScriptsHandler
{
    private readonly IFileReader _fileReader;
    private readonly ISqlExecutor _sqlExecutor;

    public ExecuteSqlScriptsHandler(
        IFileReader fileReader, 
        ISqlExecutor sqlExecutor)
    {
        _fileReader = fileReader;
        _sqlExecutor = sqlExecutor;
    }
    
    public async Task<IReadOnlyCollection<ScriptExecutionResult>> ExecuteFromFolderAsync(
        string folderPath, 
        string searchPattern = "*.sql")
    {
        if (!Directory.Exists(folderPath)) // Можно также вынести проверку в IFileReader
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

        var files = await _fileReader.GetFilesAsync(folderPath, searchPattern);
        var results = new List<ScriptExecutionResult>();

        foreach (var file in files)
        {
            results.Add(await ExecuteFileAsync(file));
        }

        return results;
    }
    
    public async Task<ScriptExecutionResult> ExecuteFileAsync(string filePath)
    {
        var result = new ScriptExecutionResult { FileName = Path.GetFileName(filePath) };
        try
        {
            if (!_fileReader.FileExists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var sqlScript = await _fileReader.ReadAllTextAsync(filePath);
            var rowsAffected = await _sqlExecutor.ExecuteNonQueryAsync(sqlScript);
            
            result.Success = true;
            result.RowsAffected = rowsAffected;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}