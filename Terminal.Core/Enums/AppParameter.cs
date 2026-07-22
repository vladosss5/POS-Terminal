namespace Terminal.Core.Enums;

/// <summary>
/// Названия параметров приложения хранящихся в БД.
/// </summary>
public enum AppParameter
{
    /// <summary>
    /// Проведена ли первичная настрокйка при установке терминала.
    /// </summary>
    IsInstalled,
    
    /// <summary>
    /// Серийный номер терминала.
    /// </summary>
    SerialNO111,
    
    /// <summary>
    /// Номер эмитента.
    /// </summary>
    IssuerId,
    
    /// <summary>
    /// IPv4 адрес TMS.
    /// </summary>
    TmsIp,
    
    /// <summary>
    /// Порт TMS.
    /// </summary>
    TmsPort,
    
    /// <summary>
    /// Хеш последнего установленного обновления.
    /// </summary>
    HashLastInstalledPatch
}