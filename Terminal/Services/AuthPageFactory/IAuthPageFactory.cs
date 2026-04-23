using Terminal.Dtos;
using Terminal.ViewModels.Pages;

namespace Terminal.Services.AuthPageFactory;

/// <summary>
/// Определяет фабрику для создания экземпляров AuthOperatorPageViewModel.
/// </summary>
public interface IAuthPageFactory
{
    /// <summary>
    /// Создаёт новый экземпляр AuthOperatorPageViewModel.
    /// </summary>
    /// <param name="parameters">Параметры для страницы аутентификации.</param>
    /// <returns>Экземпляр AuthOperatorPageViewModel.</returns>
    public AuthOperatorPageViewModel Create(AuthNavigationParameters parameters);
}