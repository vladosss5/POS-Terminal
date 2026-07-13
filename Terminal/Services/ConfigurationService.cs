using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Terminal.Application.Helpers;
using Terminal.Core.Entities.Models;
using Terminal.Core.Entities.Models.Settings;
using Terminal.Core.Entities.Models.SettingsFromPosOffice;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Services;

/// <summary>
/// Реализация сервиса для работы с конфигурацией приложения.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    /// <inheritdoc cref="ILogger" />
    private readonly ILogger<ConfigurationService> _logger;

    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;

    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new() 
    { 
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };
    
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

    /// <summary>
    /// Флаг, указывающий, что настройки загружены.
    /// </summary>
    private bool _isLoaded;

    /// <summary>
    /// Настройки приложения изменены, но не сохранены.
    /// </summary>
    private bool _appSettingsIsChanged;
    
    /// <summary>
    /// Объект для синхронизации потоков.
    /// </summary>
    private readonly Lock _lockForSettings = new();

    /// <summary>
    /// Внутреннее поле для хранения настроек.
    /// </summary>
    private SettingsModel? _currentSetting;

    
    /// <summary>
    /// Путь к файлу настроек из PosOffice.
    /// </summary>
    private const string SettingFromPosOfficeFileName = "SettingsFromPosOffice.xml";
    
    /// <summary>
    /// Путь к конфигурации в файловой системе.
    /// </summary>
    private readonly string _settingFromPosOfficeFilePath;
    
    /// <summary>
    /// Настройки из PosOffice загружены?
    /// </summary>
    private bool _isLoadedSettingsFromPosOffice;

    /// <summary>
    /// Настройки из PosOffice изменены?
    /// </summary>
    private bool _settingsFromPosOfficeIsChanged;
    
    /// <summary>
    /// Объект для синхронизации потоков.
    /// </summary>
    private readonly Lock _lockForSettingsFromPosOffice = new();

    /// <summary>
    /// Настройки из PosOffice.
    /// </summary>
    private SettingsFromPosOffice? _settingsFromPosOffice;
    
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ConfigurationService(
        ILogger<ConfigurationService> logger, 
        ITmsClient tmsClient)
    {
        _logger = logger;
        _tmsClient = tmsClient;

        var baseDirectory = OperatingSystem.IsAndroid() 
            ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) 
            : AppContext.BaseDirectory;

        _configFilePath = Path.Combine(baseDirectory, ConfigFileName);
        _settingFromPosOfficeFilePath = Path.Combine(baseDirectory, SettingFromPosOfficeFileName);
        _tableToSendFilePath = Path.Combine(baseDirectory, TableToSendFileName);
    }

    /// <inheritdoc/>
    public SettingsModel CurrentSetting
    {
        get => _isLoaded && _currentSetting != null 
            ? _currentSetting 
            : GetSettingsModel();
        set
        {
            lock (_lockForSettings)
            {
                _appSettingsIsChanged = true;
                _currentSetting = value;
                _isLoaded = true;
                SaveSettingsToFile();
            }
        }
    }

    /// <inheritdoc/>
    public SettingsFromPosOffice SettingsFromPosOffice
    {
        get => _isLoadedSettingsFromPosOffice && _settingsFromPosOffice != null 
            ? _settingsFromPosOffice 
            : GetSettingsFromPosOffice();
        set
        {
            lock (_lockForSettingsFromPosOffice)
            {
                _settingsFromPosOffice = value;
                _isLoadedSettingsFromPosOffice = true;
                SaveSettingsToFile();
            }
        }
    }

    /// <inheritdoc/>
    public async Task UpdateSettingsFromPosOffice()
    {
        var response = await _tmsClient.GetConfigurationAsync(SettingsType.Config);
        if (response == null) 
            return;
        
        var decodedString = Base64Helper.DecodeFromBase64(response.Value);
        if (string.IsNullOrEmpty(decodedString)) 
            return;
        
        var configString = decodedString
            .Replace(">False<", ">false<")
            .Replace(">True<", ">true<");
        
        var serializer = new XmlSerializer(typeof(SettingsFromPosOffice));
        using var reader = new StringReader(configString);
        _settingsFromPosOffice = (SettingsFromPosOffice)(serializer.Deserialize(reader) ?? new SettingsFromPosOffice());

        await _tmsClient.SendConfirmationUpdatingAsync([response.PosSettingsKey]);
    }

    /// <inheritdoc/>
    public void SaveSettingsToFile()
    {
        if (_appSettingsIsChanged && _currentSetting != null)
        {
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
            finally
            {
                _appSettingsIsChanged = false;
            }
        }

        if (_settingsFromPosOfficeIsChanged)
        {
            try
            {
                var directory = Path.GetDirectoryName(_settingFromPosOfficeFilePath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                
                var serializer = new XmlSerializer(typeof(SettingsFromPosOffice));
                using var stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, _settingsFromPosOffice);
                var xml = stringWriter.ToString();
                File.WriteAllText(_settingFromPosOfficeFilePath, xml);
            }
            catch (Exception e)
            {
                _logger.LogError($"Не удалось сохранить конфигурацию: {e.InnerException}");
            }
            finally
            {
                _settingsFromPosOfficeIsChanged = false;
            }
        }
    }

    /// <inheritdoc/>
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
    /// Загрузить из файла настройки приложения.
    /// </summary>
    /// <returns>Настройки.</returns>
    private SettingsModel GetSettingsModel()
    {
        lock (_lockForSettings)
        {
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
                
                _isLoaded = true;
            }
            catch (Exception)
            {
                _currentSetting = new SettingsModel();
                _isLoaded = true;
            }
        }

        return _currentSetting;
    }

    /// <summary>
    /// Загрузить из файла настройки из PosOffice.
    /// </summary>
    /// <returns>Настройки.</returns>
    private SettingsFromPosOffice GetSettingsFromPosOffice()
    {
        lock (_lockForSettingsFromPosOffice)
        {
            try
            {
                if (!File.Exists(_settingFromPosOfficeFilePath) || IsDebugBuild())
                    CopyConfigFromResources(SettingFromPosOfficeFileName, _settingFromPosOfficeFilePath);

                if (File.Exists(_settingFromPosOfficeFilePath))
                {
                    var xmlFromFile = File
                        .ReadAllText(_settingFromPosOfficeFilePath)
                        .Replace(">False<", ">false<")
                        .Replace(">True<", ">true<");
                    
                    var serializer = new XmlSerializer(typeof(SettingsFromPosOffice));
                    using var reader = new StringReader(xmlFromFile);
                        
                    _settingsFromPosOffice = (SettingsFromPosOffice)
                        (serializer.Deserialize(reader) ?? new SettingsFromPosOffice());
                }

                _isLoadedSettingsFromPosOffice = true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка загрузки настроек из PosOffice {e.Message}");
                _isLoaded = false;
            }
        }
            
        return _settingsFromPosOffice!;
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