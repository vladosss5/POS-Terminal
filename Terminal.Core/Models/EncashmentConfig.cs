namespace Terminal.Core.Models;

public class EncashmentConfig
{
    /// <summary>
    /// Путь к базе данных терминала
    /// </summary>
    public string DatabasePath { get; set; } = "terminal.db";
    
    /// <summary>
    /// Путь для сохранения исходящих данных
    /// </summary>
    public string OutPath { get; set; } = "tms/outgoing";
    
    /// <summary>
    /// Путь для входящих данных
    /// </summary>
    public string InPath { get; set; } = "tms/incoming";
    
    /// <summary>
    /// Путь к обновлениям
    /// </summary>
    public string UpdatePath { get; set; } = "tms/updates";
    
    /// <summary>
    /// Шифровать ли данные
    /// </summary>
    public bool EncryptData { get; set; } = true;
    
    /// <summary>
    /// Пароль для zip архивов
    /// </summary>
    public string ZipPassword { get; set; } = "JfYl34Igf8";
}