using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.Services;

/// <summary>
/// Реализация сервиса для работы с конфигурацией приложения.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    /// <inheritdoc cref="ILogger" />
    private readonly ILogger<ConfigurationService> _logger;
    
    /// <summary>
    /// Секции конфигурации.
    /// </summary>
    private readonly Dictionary<string, object> _sections = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        LoadConfiguration();
    }
    
    /// <summary>
    /// Загрузить конфигурацию.
    /// </summary>
    private void LoadConfiguration()
    {
        try
        {
            var assembly = typeof(ConfigurationService).Assembly;
            var stream = assembly.GetManifestResourceStream("appsettings.json");
            
            if (stream == null)
            {
                var mainAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "Terminal");
                    
                if (mainAssembly != null)
                    stream = mainAssembly.GetManifestResourceStream("appsettings.json");
            }

            if (stream == null)
            {
                _logger.LogWarning("Configuration file appsettings.json not found in embedded resources");
                return;
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (config == null)
                return;
                    
            foreach (var kvp in config)
                _sections[kvp.Key] = kvp.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration");
        }
    }
    
    /// <inheritdoc/>
    public T? GetSection<T>(string sectionName) where T : class
    {
        if (!_sections.TryGetValue(sectionName, out var section))
            return null;

        var json = JsonSerializer.Serialize(section);
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <inheritdoc/>
    public IEnumerable<PaymentTypeSetting>? GetPaymentTypeSettings()
    {
        var paymentTypes = GetSection<List<PaymentTypeSetting>>("PaymentTypes");

        return paymentTypes;
    }
}