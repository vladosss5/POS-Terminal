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
    private const string ConfigFilePath = "appsettings.json";
    
    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Словарь настроек приложения.
    /// </summary>
    private Dictionary<string, JsonElement> _config = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ConfigurationService()
    {
        _ = LoadAsync();
    }
    
    /// <inheritdoc/>
    public async Task LoadAsync()
    {
        var assembly = typeof(ConfigurationService).Assembly;
        var stream = assembly.GetManifestResourceStream(ConfigFilePath);

        
        if (stream == null)
        {
            var mainAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Terminal");
                    
            if (mainAssembly != null)
                stream = mainAssembly.GetManifestResourceStream(ConfigFilePath);
        }

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        _config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
    }
    
    /// <inheritdoc/>
    public async Task SaveAsync()
    {
        Stream? stream;
    
        var assembly = typeof(ConfigurationService).Assembly;
        stream = assembly.GetManifestResourceStream(ConfigFilePath);
    
        if (stream == null)
        {
            var mainAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Terminal");
                    
            if (mainAssembly != null)
                stream = mainAssembly.GetManifestResourceStream(ConfigFilePath);
        }
    
        if (stream != null)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "config_temp.json");
        
            await using var fileStream = File.Create(tempPath);
            await JsonSerializer.SerializeAsync(fileStream, _config, _jsonOptions);
        
            return;
        }
    
        var directory = Path.GetDirectoryName(ConfigFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await using var fileStream2 = File.Create(ConfigFilePath);
        await JsonSerializer.SerializeAsync(fileStream2, _config, _jsonOptions);
    }
    
    /// <inheritdoc/>
    public Task<T?> GetValueAsync<T>(string key, T? defaultValue = default)
    {
        if (_config.TryGetValue(key, out var value) && value is JsonElement element)
        {
            return Task.FromResult(element.Deserialize<T>() ?? defaultValue);
        }
        
        return Task.FromResult(defaultValue);
    }
    
    /// <inheritdoc/>
    public async Task SetValueAsync<T>(string key, T value)
    {
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _config[key] = jsonElement;

        await SaveAsync();
    }
}