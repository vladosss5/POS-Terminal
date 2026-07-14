using Terminal.Core.Enums;

namespace Terminal.Core.Interfaces;

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
    /// Сменить базовый адрес.
    /// </summary>
    /// <param name="address"></param>
    public void ChangeBaseAddress(string address);
    
    /// <summary>
    /// Аутентификация клиента в TMS.
    /// </summary>
    /// <param name="authData">Данные аутентификации.</param>
    public Task AuthenticationAsync(string authData);

    /// <summary>
    /// Отправить Get запрос на TMS.
    /// </summary>
    /// <param name="path">Относительный путь до конечной точки.</param>
    /// <returns>Http ответ.</returns>
    public Task<HttpResponseMessage> GetAsync(string path);

    /// <summary>
    /// Отправить Post запрос на TMS.
    /// </summary>
    /// <param name="path">Относительный путь до конечной точки.</param>
    /// <param name="content">Строковый контент.</param>
    /// <returns>Http ответ.</returns>
    public Task<HttpResponseMessage> PostAsync(string path, StringContent content);

    /// <summary>
    /// Отправить Post запрос на TMS.
    /// </summary>
    /// <param name="path">Относительный путь до конечной точки.</param>
    /// <param name="content">Составной контент.</param>
    /// <returns>Http ответ.</returns>
    public Task<HttpResponseMessage> PostAsync(string path, MultipartFormDataContent content);
}