using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using Terminal.Core.Models.Settings;

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
    /// Название файла таблиц выгружаемых в TMS.
    /// </summary>
    private const string TableToSendFileName = "TablesToSend.json";
    
    /// <summary>
    /// Путь к файлу таблиц выгружаемых в TMS.
    /// </summary>
    private readonly string _tableToSendFilePath;

    private readonly ILogger<ConfigurationService> _logger;
    
    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new() 
    { 
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Флаг, указывающий, что настройки загружены.
    /// </summary>
    private bool _isLoaded;
    
    /// <summary>
    /// Объект для синхронизации потоков.
    /// </summary>
    private readonly Lock _lock = new();

    /// <summary>
    /// Внутреннее поле для хранения настроек.
    /// </summary>
    private SettingsModel? _currentSetting;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        var baseDirectory = OperatingSystem.IsAndroid() 
            ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) 
            : AppContext.BaseDirectory;

        _configFilePath = Path.Combine(baseDirectory, ConfigFileName);
        _tableToSendFilePath = Path.Combine(baseDirectory, TableToSendFileName);
    }

    public SettingsModel CurrentSetting
    {
        get
        {
            if (!_isLoaded)
                LoadSettings();

            return _currentSetting ?? new SettingsModel();
        }
        set
        {
            lock (_lock)
            {
                _currentSetting = value;
                _isLoaded = true;
                SaveSettingsToFile();
            }
        }
    }


    
    /// <summary>
    /// Загрузить настройки из файла.
    /// </summary>
    private void LoadSettings()
    {
        lock (_lock)
        {
            if (_isLoaded)
                return;
            
            try
            {
                if (!File.Exists(_configFilePath) || IsDebugBuild())
                    CopyConfigFromResources(ConfigFileName, _configFilePath);
                
                if (File.Exists(_configFilePath))
                {
                    var jsonFromFile = File.ReadAllText(_configFilePath);
                    _currentSetting = JsonSerializer.Deserialize<SettingsModel>(jsonFromFile, _jsonOptions);
                }
                
                _currentSetting ??= new SettingsModel();
                _currentSetting.PaymentTypes ??= [];
                _currentSetting.Organisation ??= new SettingOrganisation();
                
                _isLoaded = true;
            }
            catch (Exception)
            {
                _currentSetting = new SettingsModel();
                _isLoaded = true;
            }
        }
    }
    
    /// <summary>
    /// Сохранить настройки в файл.
    /// </summary>
    public void SaveSettingsToFile()
    {
        if (_currentSetting == null)
            return;
        
        try
        {
            var directory = Path.GetDirectoryName(_configFilePath);
            
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            var json = JsonSerializer.Serialize(_currentSetting, _jsonOptions);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception e)
        {
            _logger.LogError($"Не удалось сохранить конфигурацию: {e.InnerException}");
        }
    }

    public List<TableToSendDto> GetTablesToSend()
    {
        if (!File.Exists(_tableToSendFilePath) || IsDebugBuild())
            CopyConfigFromResources(TableToSendFileName, _tableToSendFilePath);

        List<TableToSendDto> tables = [];

        if (!File.Exists(_tableToSendFilePath)) 
            return tables;
        
        var jsonFromFile = File.ReadAllText(_tableToSendFilePath);
        var tablesInFile = JsonSerializer.Deserialize<TableToSendDto[]>(jsonFromFile, _jsonOptions);

        if (tablesInFile != null)
            tables.AddRange(tablesInFile);

        return tables;
    }

    /// <summary>
    /// Скопировать файл конфигурации из ресурсов в файловую систему.
    /// </summary>
    private void CopyConfigFromResources(string fileName, string filePath)
    {
        Stream? stream = null;
    
        var assembly = typeof(ConfigurationService).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();
    
        var resourceName = Array.Find(resourceNames, r => 
            r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
    
        if (resourceName != null)
            stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null) 
            return;
        
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using var fileStream = File.Create(filePath);
        stream.CopyTo(fileStream);
    }
    
    private static bool IsDebugBuild()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}