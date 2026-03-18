using System.Collections.Generic;
using System.IO;
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
    private Dictionary<string, object> _config = new();

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
        if (!File.Exists(ConfigFilePath))
        {
            _config = new Dictionary<string, object>();
            return;
        }

        await using var stream = File.OpenRead(ConfigFilePath);
        
        _config = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(stream)
                  ?? new Dictionary<string, object>();
    }
    
    /// <inheritdoc/>
    public async Task SaveAsync()
    {
        var directory = Path.GetDirectoryName(ConfigFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(ConfigFilePath);
        await JsonSerializer.SerializeAsync(stream, _config, _jsonOptions);
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