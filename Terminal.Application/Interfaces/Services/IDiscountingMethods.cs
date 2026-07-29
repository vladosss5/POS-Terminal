using Terminal.Application.Dtos.CardInfoRoot;
using Terminal.Application.Dtos.DebitRoot;
using Terminal.Application.Dtos.DiscountRoot;

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
    public CardInfoDtoResponseDto GetCardInfo(CardInfoDtoRequestDto dtoRequestDto);

    /// <summary>
    /// Предварительно расчиать данные продажи.
    /// </summary>
    /// <param name="requestDto">Dto запроса.</param>
    /// <returns>Dto ответа.</returns>
    public DiscountResponseDto CalculateDiscount(DiscountRequestDto requestDto);

    public DebitResponseDto Debit(DebitRequestDto requestDto);
}