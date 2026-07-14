using System.Xml.Serialization;
using Terminal.Application.Helpers;
using Terminal.Core.Entities.Models.SettingsFromPosOffice;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class ConfigurationUpdatingService : IConfigurationUpdatingService
{
    /// <inheritdoc cref="ITmsService" />
    private readonly ITmsService _tmsService;

    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ConfigurationUpdatingService(
        ITmsService tmsService, 
        IConfigurationService configurationService)
    {
        _tmsService = tmsService;
        _configurationService = configurationService;
    }

    /// <inheritdoc/>
    public async Task UpdateSettingsFromPosTms()
    {
        var response = await _tmsService.GetConfigurationAsync(SettingsType.Config);
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
        
        _configurationService.SettingsFromPosOffice = (SettingsFromPosOffice)(serializer.Deserialize(reader) ?? new SettingsFromPosOffice());
        
        await _tmsService.SendConfirmationUpdatingAsync([response.PosSettingsKey]);
    }
}