using Terminal.Core.Enums;
using Terminal.Core.Models;

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
    /// Получить предыдущие результаты инкассаций. 
    /// </summary>
    /// <param name="terminalId">Номер терминала.</param>
    /// <returns>Массив байт архива.</returns>
    public Task<byte[]> GetResultsEncashmentCollectionAsync(string terminalId);

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
}