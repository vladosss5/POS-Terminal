using Terminal.Application.Dtos.CardInfoRoot;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Методы работы с библиотекой скидок.
/// </summary>
public interface IDiscountingMethods
{
    /// <summary>
    /// Получить информацию о карте.
    /// </summary>
    /// <param name="dtoRequestDto">Запрос информации.</param>
    /// <returns>Ответ с информацией.</returns>
    public Task<CardInfoDtoResponseDto> GetCardInfoAsync(CardInfoDtoRequestDto dtoRequestDto);
}