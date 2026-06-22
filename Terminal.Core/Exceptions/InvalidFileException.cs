namespace Terminal.Core.Exceptions;

/// <summary>
/// Некорректный файл.
/// </summary>
/// <param name="message">Сообщение об ошибке.</param>
public class InvalidFileException(string message) : Exception(message);