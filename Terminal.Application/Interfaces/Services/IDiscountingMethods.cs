using Terminal.Application.Dtos;

namespace Terminal.Application.Interfaces.Services;

public interface IDiscountingMethods
{
    public Task<CardInfoResponseDto> GetCardInfoAsync(CardInfoRequestDto requestDto);
}