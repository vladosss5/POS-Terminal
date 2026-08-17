using Terminal.Application.Dtos;
using Terminal.Application.Dtos.CardInfoRoot;
using Terminal.Application.Dtos.DebitRoot;
using Terminal.Application.Dtos.DiscountRoot;
using Terminal.Core.Exceptions.ProcessingCenter;

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
    /// Предварительно рассчитать данные продажи.
    /// </summary>
    /// <param name="requestDto">Dto запроса.</param>
    /// <returns>Dto ответа.</returns>
    public DiscountResponseDto CalculateDiscount(DiscountRequestDto requestDto);

    /// <summary>
    /// Дебетование карты.
    /// </summary>
    /// <param name="requestDto">Dto запроса.</param>
    /// <returns>Dto ответа.</returns>
    public DebitResponseDto Debit(DebitRequestDto requestDto);
    
    /// <summary>
    /// В ответе от ПЦ ищет ошибки и вызывает внутренние exceptions.
    /// </summary>
    /// <param name="requestDto">Блок Request из ответа от ПЦ.</param>
    /// <exception cref="RequiredParameterException">В запросе требуются дополнительные параметры.</exception>
    /// <exception cref="RequiredPinCodeException">Требуется ввод ПИН-кода карты.</exception>
    /// <exception cref="AmountException">Запрошенное кол-во больше доступного.</exception>
    public void CheckErrorIntoResponse(RequestDto requestDto);
}