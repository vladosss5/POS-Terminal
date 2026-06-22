namespace Terminal.Core.Exceptions;

/// <summary>
/// Объект не найден.
/// </summary>
/// <param name="message">Сообщение.</param>
public class NotFoundException(string message) : Exception(message);