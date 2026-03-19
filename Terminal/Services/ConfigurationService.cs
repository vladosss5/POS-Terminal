using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Services;

/// <summary>
/// Реализация сервиса для работы с конфигурацией приложения.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    /// <summary>
    /// Относительный путь к файлу конфигурации
    /// </summary>
    private const string ConfigFileName = "appsettings.json";
    
    /// <summary>
    /// Путь к конфигурации в файловой системе.
    /// </summary>
    private readonly string _configFilePath;
    
    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Словарь настроек приложения.
    /// </summary>
    private Dictionary<string, JsonElement>? _config = new();
    
    private readonly Lazy<Task> _initialization;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ConfigurationService()
    {
        _configFilePath = GetConfigFilePath();
        
        _initialization = new Lazy<Task>(LoadAsync);
    }
    
    /// <inheritdoc/>
    public async Task<T?> GetValueAsync<T>(string key, T? defaultValue = default)
    {
        await _initialization.Value;
        
        if (_config!.TryGetValue(key, out var value))
            return value.Deserialize<T>() ?? defaultValue;
        
        return defaultValue;
    }
    
    /// <inheritdoc/>
    public async Task SetValueAsync<T>(string key, T value)
    {
        await _initialization.Value;
        
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _config![key] = jsonElement;

        await SaveToFileSystemAsync();
    }
    
    /// <summary>
    /// Загрузить конфигурацию из файла.
    /// </summary>
    private async Task LoadAsync()
    {
        if (!File.Exists(_configFilePath))
        {
            await CopyConfigFromResourcesAsync();
        }
        
        if (File.Exists(_configFilePath))
        {
            var jsonFromFile = await File.ReadAllTextAsync(_configFilePath);
            _config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonFromFile);
        }
        else
        {
            _config = new Dictionary<string, JsonElement>();
            await SaveToFileSystemAsync();
        }
    }
    
    /// <summary>
    /// Сохранить конфигурацию в файловую систему для последующей работы.
    /// </summary>
    private async Task SaveToFileSystemAsync()
    {
        var directory = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(_config, _jsonOptions);
        await File.WriteAllTextAsync(_configFilePath, json);
    }
    
    /// <summary>
    /// Получить путь к фалу конфигурации из сборки.
    /// </summary>
    /// <returns>Путь к файлу.</returns>
    private static string GetConfigFilePath()
    {
        var baseDirectory = OperatingSystem.IsAndroid() 
            ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) 
            : AppContext.BaseDirectory;

        return Path.Combine(baseDirectory, ConfigFileName);
    }
    
    /// <summary>
    /// Скопировать файл конфигурации из ресурсов в файловую систему.
    /// </summary>
    private async Task CopyConfigFromResourcesAsync()
    {
        Stream? stream = null;
    
        var assembly = typeof(ConfigurationService).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();
    
        var resourceName = resourceNames.FirstOrDefault(r => 
            r.EndsWith(ConfigFileName, StringComparison.OrdinalIgnoreCase));
    
        if (resourceName != null)
        {
            stream = assembly.GetManifestResourceStream(resourceName);
        }
    
        if (stream != null)
        {
            var directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await using var fileStream = File.Create(_configFilePath);
            await stream.CopyToAsync(fileStream);
        }
    }
}