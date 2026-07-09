using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Core.TmsDtos.TerminalUpdate;

namespace Terminal.Persistence.TmsClient;

/// <summary>
/// Клиент взаимодействия с TMS.
/// </summary>
public interface ITmsClient
{
    /// <summary>
    /// Статус подключения.
    /// </summary>
    public TmsConnectionStatus ConnectionStatus { get; }

    /// <summary>
    /// Аутентификация клиента в TMS.
    /// </summary>
    /// <param name="authData">Данные аутентификации.</param>
    public Task AuthenticationAsync(string authData);

    /// <summary>
    /// Получить конфигурацию.
    /// </summary>
    /// <param name="settingType">Тип обновляемой настройки.</param>
    /// <returns>Base64 строка с данными.</returns>
    public Task<TerminalUpdateResponseDto?> GetConfigurationAsync(SettingsType settingType);

    /// <summary>
    /// Отправить подтверждение обновлений.
    /// </summary>
    /// <param name="updatedSettingIds">Идентификаторы обновлённых настроек.</param>
    public Task SendConfirmationUpdatingAsync(int[] updatedSettingIds);

    /// <summary>
    /// Получить предыдущие результаты инкассаций. 
    /// </summary>
    /// <returns>Массив байт архива.</returns>
    public Task<byte[]> GetResultsEncashmentCollectionAsync();

    /// <summary>
    /// Отправить пакет сжатой информации из БД на TMS.
    /// </summary>
    /// <param name="data">Массив байт сжатого файла.</param>
    /// <param name="table">Таблица данные которой принадлежат.</param>
    /// <param name="fileName">Название файла.</param>
    /// <param name="recordCount">Кол-во пакетов.</param>
    /// <returns>Успешность передачи.</returns>
    public Task<bool> SendEncashmentTablesAsync(byte[] data, TableToSendDto table, string fileName, int recordCount);

    /// <summary>
    /// Запустить на сервере процесс инкассации данных из переданных файлов.
    /// </summary>
    public Task StartEncashmentOnTmsAsync();

    /// <summary>
    /// Скачать пакет обновлений с TMS. 
    /// </summary>
    /// <returns>Файл в виде потока.</returns>
    public Task<(Stream, string)> DownloadUpdatingFileAsync();

    /// <summary>
    /// Отправить подтверждение окончания скачивания пакета обновлений с TMS.
    /// </summary>
    public Task ConfirmReceiptUpdatingFileAsync();

    /// <summary>
    /// Получить номер версии пакета обновлений из TMS.
    /// </summary>
    /// <returns>Номер версии или пустая строка, если обновления нет.</returns>
    public Task<string?> GetNumberNewVersion();
}